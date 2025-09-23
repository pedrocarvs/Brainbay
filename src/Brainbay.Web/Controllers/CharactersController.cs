using Microsoft.AspNetCore.Mvc;
using Brainbay.Core.Models;
using Brainbay.Application;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Brainbay.Web.Controllers;

public class CharactersController : Controller
{
    private readonly ICharacterService _characterService;

    public CharactersController(ICharacterService characterService)
    {
        _characterService = characterService;
    }

    public async Task<IActionResult> Index()
    {
        var (characters, fromDb) = await _characterService.GetAllCharactersAsync();
        Response.Headers["from-database"] = fromDb.ToString().ToLower();
        return View(characters);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Character model)
    {
        if (!ModelState.IsValid) return View(model);

        await _characterService.AddCharacterAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("characters/planet/{planet}")] 
    public async Task<IActionResult> ByPlanet(string planet)
    {
        var list = await _characterService.GetCharactersByPlanetAsync(planet);

        Response.Headers["from-database"] = "true";

        return View("Index", list);
    }
}