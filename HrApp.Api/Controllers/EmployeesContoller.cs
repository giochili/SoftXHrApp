using HrApp.Core.Entities;
using HrApp.Core.Enums;
using HrApp.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HrApp.Api.Controllers
{
    [Route("api/Employees")]
    [ApiController]
    [Authorize]
    public class EmployeesContoller : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPositionRepository _positionRepository;

        public EmployeesContoller(IEmployeeRepository employeeRepository, IPositionRepository positionRepository)
        {
            _employeeRepository = employeeRepository;
            _positionRepository = positionRepository;
        }

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
        public async Task<IActionResult> Create([FromBody] Employee model)
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

            model.Status = EmployeeStatus.Inactive; // required by spec
            model.IsActive = false;

            var created = await _employeeRepository.AddAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Employee model)
        {
            var existing = await _employeeRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            if (model.PersonalNumber == null || model.PersonalNumber.Length != 11 || !model.PersonalNumber.All(char.IsDigit))
                return BadRequest(new { Success = false, Message = "პირადი ნომერი უნდა იყოს 11 ციფრი" });

            if (await _employeeRepository.ExistsByPersonalNumberOrEmailAsync(model.PersonalNumber, model.Email, excludeId: id))
                return BadRequest(new { Success = false, Message = "ასეთი თანამშრომელი უკვე არსებობს" });

            existing.PersonalNumber = model.PersonalNumber;
            existing.FirstName = model.FirstName;
            existing.LastName = model.LastName;
            existing.Gender = model.Gender;
            existing.BirthDate = model.BirthDate;
            existing.Email = model.Email;
            existing.PositionId = model.PositionId;
            existing.Status = model.Status;
            existing.DismissalDate = model.DismissalDate;

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
