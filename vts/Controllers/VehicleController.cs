using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using vts.Data;
using vts.Models;
using vts.Repositories;

namespace vts.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountRepository _accountRepository;

        public VehicleController(ApplicationDbContext context, IAccountRepository accountRepository)
        {
            _context = context;
            _accountRepository = accountRepository;
        }
        public IActionResult AddVehicle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle([Bind("Type,Brand,Model,LicensePlate,Transmission,Color,FuelType,Year,Horsepower,EngineCapacity,Kilometer")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var currentUser = User;
                var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;
                var user = await _accountRepository.FindByEmailAsync(currentUserEmail);
                vehicle.UserId = user.Id;

                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShowVehicle));
            }
            return View(vehicle);
        }

        public IActionResult EditVehicle(int vId)
        {
            var vehicle = _context.Vehicles.Where(v => v.VehicleId == vId).FirstOrDefault();
            return View(vehicle);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditVehicle([Bind("Color,Horsepower,EngineCapacity,Kilometer")] Vehicle vehicle, int vId)
        {
            if (ModelState.IsValid)
            {
                var tempVehicle = _context.Vehicles.Where(v => v.VehicleId == vId).FirstOrDefault();
                tempVehicle.Color = vehicle.Color;
                tempVehicle.Horsepower = vehicle.Horsepower;
                tempVehicle.EngineCapacity = vehicle.EngineCapacity;
                tempVehicle.Kilometer = vehicle.Kilometer;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShowVehicle));
            }
            return View(vehicle);
        }

        public async Task<IActionResult> DeleteVehicle(int vId)
        {
            var vehicle = _context.Vehicles.Where(v => v.VehicleId == vId).FirstOrDefault();
            _context.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowVehicle");
        }

        public async Task<IActionResult> ShowVehicle()
        {
            var currentUser = User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;
            var user = await _accountRepository.FindByEmailAsync(currentUserEmail);
            
            var vehicleList = _context.Vehicles.Where(v => v.UserId == user.Id).ToList();
            user.Vehicles = vehicleList;

            return View(user);
        }

        public async Task<IActionResult> FilterVehicle(string vType)
        {
            var currentUser = User;
            var currentUserEmail = currentUser.FindFirst(ClaimTypes.Email).Value;
            var user = await _accountRepository.FindByEmailAsync(currentUserEmail);

            var vehicleList = _context.Vehicles.Where(v => v.UserId == user.Id && vType == v.Type).ToList();
            user.Vehicles = vehicleList;

            return View("ShowVehicle", user);
        }

        public IActionResult VehicleDetails(int vId)
        {
            var vehicle = _context.Vehicles.Where(v => v.VehicleId == vId).FirstOrDefault();
            var vList = _context.Maintenances.Where(m => m.VehicleId == vId).ToList();
            vehicle.Maintenances = vList;

            return View(vehicle);
        }
        public IActionResult AddMaintenance(int vId)
        {
            var maintenance = new Maintenance();
            maintenance.VehicleId = vId;
            return View(maintenance);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddMaintenance([Bind("MaintenanceKilometer,Cost,Date,Done,NextMaintenanceDate,NextMaintenanceKilometer,Note")] Maintenance maintenance, int vId)
        {
            if (ModelState.IsValid)
            {
                var vehicle = _context.Vehicles.Where(v => v.VehicleId == vId).FirstOrDefault();
                maintenance.VehicleId = vId;
                _context.Maintenances.Add(maintenance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ShowVehicle));
            }
            return View();
        }

        public async Task<IActionResult> DeleteMaintenance(int mId)
        {
            var maintenance = _context.Maintenances.Where(m => m.MaintenanceId == mId).FirstOrDefault();
            _context.Remove(maintenance);
            await _context.SaveChangesAsync();
            return RedirectToAction("ShowVehicle");
        }

        public IActionResult MaintenanceDetails(int mId)
        {
            var maintenance = _context.Maintenances.Where(m => m.MaintenanceId == mId).FirstOrDefault();
            return View(maintenance);
        }
    }
}
