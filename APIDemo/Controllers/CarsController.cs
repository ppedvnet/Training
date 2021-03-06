﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using APIDemo.Models;
using APIDemo.Services;
using APIDemo.Dtos;
using APIDemo.Helpers;

namespace APIDemo.Controllers
{
    //[Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Teacher")]
    public class CarsController : Controller
    {
        private readonly ICarsRepository _repo;
        private readonly IMapper _mapper;

        public CarsController(ICarsRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET: api/Cars
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var cars = await _repo.GetAll(new ResourceParameters());

            var carsDto = _mapper.Map<IList<CarDto>>(cars);

            return Ok(carsDto);
        }

        // GET: api/Cars/5
        [HttpGet("{id}", Name = "GetCar")]
        public async Task<ActionResult<CarDto>> GetCarById(int id)
        {
            var car = await _repo.FindById(id);
            if (car == null)
            {
                return NotFound();
            }

            var carDto = _mapper.Map<CarDto>(car);

            return carDto;
        }

        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCar(int id, [FromBody] Car car)
        {
            if (car == null || car.Id != id)
            {
                return BadRequest();
            }
            var carToUpdate = await _repo.FindById(id);
            if (carToUpdate == null)
            {
                return NotFound();
            }

            carToUpdate.BrandName = car.BrandName;
            carToUpdate.ModelName = car.ModelName;
            carToUpdate.YearOfConstruction = car.YearOfConstruction;

            await _repo.SaveAll();
            return new NoContentResult();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Car>> Patch(int id, JsonPatchDocument<Car> patch)
        {
            var car = await _repo.FindById(id);
            if (car == null)
            {
                return NotFound();
            }

            patch.ApplyTo(car, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _repo.SaveAll();

            return car;
        }

        // POST: api/Cars
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar([FromBody] Car car)
        {
            if (car == null) return BadRequest();

            _repo.Add(car);
            await _repo.SaveAll();

            return CreatedAtRoute("GetCar", new { id = car.Id }, car);
        }

        // DELETE: api/Cars/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCar(int id)
        {
            var car = await _repo.FindById(id);
            if (car == null)
            {
                return NotFound();
            }
            _repo.Remove(car);
            await _repo.SaveAll();

            return new NoContentResult();
        }
    }
}