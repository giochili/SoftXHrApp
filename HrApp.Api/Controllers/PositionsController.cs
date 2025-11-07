using System.Collections.Generic;
using System.Linq;
using HrApp.Core.Entities;
using HrApp.Core.Interfaces;
using HrApp.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace HrApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PositionsController : ControllerBase
    {
        private readonly IPositionRepository _positionRepository;

        public PositionsController(IPositionRepository positionRepository)
        {
            _positionRepository = positionRepository;
        }

        [HttpGet]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _positionRepository.ListAsync();
            return Ok(positions);
        }

        [HttpGet("hierarchy")]
        public async Task<IActionResult> GetHierarchy()
        {
            var roots = await _positionRepository.GetHierarchyAsync();
            var dto = roots.Select(MapToTreeDto).ToList();
            return Ok(dto);
        }

        private static PositionTreeDto MapToTreeDto(Position position)
        {
            return new PositionTreeDto
            {
                Id = position.Id,
                Title = position.Title,
                ParentPositionId = position.ParentPositionId,
                Children = (position.SubPositions ?? new List<Position>())
                    .Select(MapToTreeDto)
                    .ToList()
            };
        }

        [HttpPost]
        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Position model)
        {
            var created = await _positionRepository.AddAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pos = await _positionRepository.GetByIdAsync(id);
            if (pos == null) return NotFound();
            return Ok(pos);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Position model)
        {
            var existing = await _positionRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            existing.Title = model.Title;
            existing.ParentPositionId = model.ParentPositionId;
            await _positionRepository.UpdateAsync(existing);
            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _positionRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _positionRepository.DeleteAsync(existing);
            return NoContent();
        }
    }
}
