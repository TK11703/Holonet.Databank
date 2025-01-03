﻿
using Holonet.Databank.Core.Entities;
using Holonet.Databank.Core.Models;
using Holonet.Databank.Infrastructure.Repositories;
using System.Data;

namespace Holonet.Databank.Application.Services;
public class SpeciesService(ISpeciesRepository speciesRepository, IAuthorService authorService, IAliasRepository aliasRepository, IDataRecordService dataRecordService) : ISpeciesService
{
    private readonly ISpeciesRepository _speciesRepository = speciesRepository;
    private readonly IAuthorService _authorService = authorService;
    private readonly IAliasRepository _aliasRepository = aliasRepository;
    private readonly IDataRecordService _dataRecordService = dataRecordService;

    public async Task<Species?> GetSpeciesById(int id)
    {
        var species = await _speciesRepository.GetSpecies(id);
        if (species != null && species.AuthorId > 0)
        {
            var author = await _authorService.GetAuthorById(species.AuthorId, true);
            if (author != null)
            {
                species.UpdatedBy = author;
            }
            PopulateSpecies(species, true);
        }
        return species;
    }

    public async Task<bool> SpeciesExists(int id, string name)
    {
        bool exists = await _speciesRepository.SpeciesExists(id, name);
        return exists;
    }

    public async Task<IEnumerable<Species>> GetSpecies(bool populate = false, bool populateDataRecords = false)
    {
        if (populate)
        {
            var species = await _speciesRepository.GetSpecies();
            foreach (var item in species)
            {
                PopulateSpecies(item, populateDataRecords);
            }
            return species;
        }
        else
        {
            return await _speciesRepository.GetSpecies();
        }
    }

    public async Task<IEnumerable<Species>> GetSpecies(long utcTicks, bool populate = false, bool populateDataRecords = false)
    {
        if (populate)
        {
            var species = await _speciesRepository.GetSpecies(utcTicks);
            foreach (var item in species)
            {
                PopulateSpecies(item, populateDataRecords);
            }
            return species;
        }
        else
        {
            return await _speciesRepository.GetSpecies(utcTicks);
        }
    }

    public async Task<PageResult<Species>> GetPagedAsync(PageRequest pageRequest)
    {
        return await _speciesRepository.GetPagedAsync(pageRequest);
    }

    private void PopulateSpecies(Species species, bool populateDataRecords = false)
    {
        species.Aliases = _aliasRepository.GetAliases(speciesId: species.Id).Result;
        if (species.Aliases.Any())
        {
            species.AliasIds = species.Aliases.Select(c => c.Id);
        }
        if (populateDataRecords)
        {
            species.DataRecords = _dataRecordService.GetDataRecordsById(speciesId: species.Id).Result;
            if (species.DataRecords.Any())
            {
                species.DataRecordIds = species.DataRecords.Select(c => c.Id);
            }
        }
    }

    public async Task<int> CreateSpecies(Species species)
    {
        var exists = await SpeciesExists(0, species.Name);
        if (exists)
        {
            throw new DataException("Species already exists.");
        }
        int newId = await _speciesRepository.CreateSpecies(species);
        if (newId > 0)
        {
            await _aliasRepository.AddAliases(GetAliasTable(newId, species.Aliases), species.UpdatedBy.AzureId);
        }
        return newId;
    }

    public async Task<bool> UpdateSpecies(Species species)
    {
        var exists = await SpeciesExists(species.Id, species.Name);
        if (exists)
        {
            throw new DataException("Species already exists.");
        }
        bool updated = _speciesRepository.UpdateSpecies(species);
        if (updated)
        {
            int completedCmds = 0;
            if (await _aliasRepository.DeleteAliases(speciesId: species.Id))
            {
                completedCmds++;
                if (await _aliasRepository.AddAliases(GetAliasTable(species.Id, species.Aliases), species.UpdatedBy.AzureId))
                {
                    completedCmds++;
                }
            }
            updated = completedCmds == 2;
        }
        return updated;
    }

    public async Task<bool> DeleteSpecies(int id)
    {
        return await _speciesRepository.DeleteSpecies(id);
    }

    private static DataTable GetAliasTable(int speciesId, IEnumerable<Alias> aliases)
    {
        DataTable dt = new();
        dt.Columns.Add("AliasName", typeof(string));
        dt.Columns.Add("CharacterId", typeof(int));
        dt.Columns.Add("HistoricalEventId", typeof(int));
        dt.Columns.Add("PlanetId", typeof(int));
        dt.Columns.Add("SpeciesId", typeof(int));
        foreach (var alias in aliases)
        {
            dt.Rows.Add(alias.Name, null, null, null, speciesId);
        }
        return dt;
    }
}
