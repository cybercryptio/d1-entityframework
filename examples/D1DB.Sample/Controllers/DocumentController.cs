// Copyright 2020-2022 CYBERCRYPT

using D1DB.Sample.Models;
using Microsoft.AspNetCore.Mvc;
using D1DB.Sample.Data;
using Microsoft.EntityFrameworkCore;
using CyberCrypt.D1.EntityFramework;

namespace D1DB.Sample.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private readonly StorageContext storageContext;

    public DocumentController(StorageContext storageContext)
    {
        this.storageContext = storageContext;
    }

    [HttpGet(Name = "GetDocument")]
    public async Task<ActionResult<Document>> GetDocument(int documentID)
    {
        var document = await storageContext.Documents.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == documentID);

        if (document is not null)
        {
            return document;
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost(Name = "CreateDocument")]
    public async Task<IActionResult> CreateDocument(NewDocument newDocument)
    {
        var document = new Document { Data = newDocument.Data, AssociatedData = newDocument.AssociatedData };
        await storageContext.Documents.AddAsync(document);
        await storageContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDocument), new { id = document!.Id }, document);
    }

    [HttpDelete(Name = "DeleteDocument")]
    public async Task<IActionResult> Delete(int documentID)
    {
        var document = await storageContext.Documents.FindAsync(documentID);

        if (document is not null)
        {
            storageContext.Documents.Remove(document);
            await storageContext.SaveChangesAsync();
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost("search", Name = "SearchDocuments")]
    public async Task<ActionResult> Search(string[] keywords)
    {
        var documents = await storageContext.Documents.WhereSearchable(x => x.Data, keywords)
            .ToListAsync();

        return Ok(documents);
    }
}
