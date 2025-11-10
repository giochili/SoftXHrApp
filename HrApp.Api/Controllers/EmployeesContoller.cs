using HrApp.Core.Entities;
using HrApp.Core.Enums;
using HrApp.Core.Interfaces;
using HrApp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Quartz;

namespace HrApp.Api.Controllers
{
    [Route("api/Employees")]
    [ApiController]
    [Authorize]
    public class EmployeesContoller : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPositionRepository _positionRepository;
        private readonly ISchedulerFactory _schedulerFactory;

        public EmployeesContoller(IEmployeeRepository employeeRepository, IPositionRepository positionRepository, ISchedulerFactory schedulerFactory)
        {
            _employeeRepository = employeeRepository;
            _positionRepository = positionRepository;
            _schedulerFactory = schedulerFactory;
        }

        [HttpGet]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] string? q)
        {
            var employees = await _employeeRepository.SearchByNameAsync(q);
            return Ok(employees);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeDto model)
        {
            if (model.PersonalNumber == null || model.PersonalNumber.Length != 11 || !model.PersonalNumber.All(char.IsDigit))
                return BadRequest(new { Success = false, Message = "პირადი ნომერი უნდა იყოს 11 ციფრი" });

            if (string.IsNullOrWhiteSpace(model.Email))
                return BadRequest(new { Success = false, Message = "ელ.ფოსტა სავალდებულოა" });

            if (model.PositionId <= 0)
                return BadRequest(new { Success = false, Message = "პოზიცია სავალდებულოა" });

            if (!Enum.IsDefined(typeof(EmployeeStatus), model.Status))
                return BadRequest(new { Success = false, Message = "სტატუსი სავალდებულოა" });

            if (await _employeeRepository.ExistsByPersonalNumberOrEmailAsync(model.PersonalNumber, model.Email))
                return BadRequest(new { Success = false, Message = "ასეთი თანამშრომელი უკვე არსებობს" });

            var today = DateTime.UtcNow.Date;
            var birthDate = model.BirthDate.Date;
            var minDate = today.AddYears(-100);
            if (birthDate > today)
                return BadRequest(new { Success = false, Message = "დაბადების თარიღი არ შეიძლება იყოს მომავალი" });
            if (birthDate < minDate)
                return BadRequest(new { Success = false, Message = "დაბადების თარიღი არ შეიძლება იყოს 100 წელზე უფრო ძველი" });

            var entity = new Employee
            {
                PersonalNumber = model.PersonalNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = (Gender)model.Gender,
                BirthDate = model.BirthDate,
                Email = model.Email,
                PositionId = model.PositionId,
                Status = EmployeeStatus.Inactive,
                DismissalDate = model.DismissalDate,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _employeeRepository.AddAsync(entity);

            // Schedule activation job for 1 hour after creation
            var scheduler = await _schedulerFactory.GetScheduler();
            var jobKey = new JobKey($"ActivateEmployee_{created.Id}", "EmployeeActivation");
            var job = JobBuilder.Create<HrApp.Infrastructure.Jobs.ActivateEmployeeJob>()
                .WithIdentity(jobKey)
                .UsingJobData("EmployeeId", created.Id)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"ActivateEmployeeTrigger_{created.Id}", "EmployeeActivation")
                .StartAt(DateTimeOffset.UtcNow.AddHours(1))
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEmployeeDto model)
        {
            var existing = await _employeeRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (model.PersonalNumber == null || model.PersonalNumber.Length != 11 || !model.PersonalNumber.All(char.IsDigit))
                return BadRequest(new { Success = false, Message = "პირადი ნომერი უნდა იყოს 11 ციფრი" });

            if (await _employeeRepository.ExistsByPersonalNumberOrEmailAsync(model.PersonalNumber, model.Email, excludeId: id))
                return BadRequest(new { Success = false, Message = "ასეთი თანამშრომელი უკვე არსებობს" });

            var today = DateTime.UtcNow.Date;
            var birthDate = model.BirthDate.Date;
            var minDate = today.AddYears(-100);
            if (birthDate > today)
                return BadRequest(new { Success = false, Message = "დაბადების თარიღი არ შეიძლება იყოს მომავალი" });
            if (birthDate < minDate)
                return BadRequest(new { Success = false, Message = "დაბადების თარიღი არ შეიძლება იყოს 100 წელზე უფრო ძველი" });

            // Prevent changing status to Active for fresh employees (created less than 1 hour ago and inactive)
            var isFreshEmployee = existing.CreatedAt > DateTime.UtcNow.AddHours(-1);
            if (isFreshEmployee && !existing.IsActive && model.Status == (int)EmployeeStatus.Active)
            {
                return BadRequest(new { Success = false, Message = "ახალი თანამშრომელი ავტომატურად გააქტიურდება 1 საათის შემდეგ. სტატუსის ხელით შეცვლა შეუძლებელია." });
            }

            existing.PersonalNumber = model.PersonalNumber;
            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Gender = (Gender)model.Gender;
            existing.BirthDate = model.BirthDate;
            existing.Email = model.Email;
            existing.PositionId = model.PositionId;
            existing.Status = (EmployeeStatus)model.Status;
            existing.DismissalDate = model.DismissalDate;
            
            // Sync IsActive with Status: Active status means IsActive should be true
            // For fresh employees (created less than 1 hour ago), keep IsActive false even if Status is Active
            if (existing.Status == EmployeeStatus.Active && !isFreshEmployee)
            {
                existing.IsActive = true;
            }
            else if (existing.Status != EmployeeStatus.Active)
            {
                existing.IsActive = false;
            }

            await _employeeRepository.UpdateAsync(existing);
            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _employeeRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _employeeRepository.DeleteAsync(existing);
            return NoContent();
        }
    }
}
