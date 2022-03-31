// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Services;
using EncryptonizeDBSample.Models;
using Microsoft.AspNetCore.Mvc;

namespace EncryptonizeDBSample.Controllers;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    private readonly DocumentService _service;

    public DocumentController(DocumentService service)
    {
        _service = service;
    }

    [HttpGet(Name = "GetDocument")]
    public ActionResult<Document> GetDocument(int documentID)
    {
        var document = _service.GetById(documentID);

        if(document is not null)
        {
            return document;
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost(Name = "CreateDocument")]
    public IActionResult CreateDocument(Document newDocument)
    {
        var document = _service.CreateDocument(newDocument);
        return CreatedAtAction(nameof(GetDocument), new { id = document!.ID }, document);
    }

    [HttpDelete(Name = "DeleteDocument")]
    public IActionResult Delete(int documentID)
    {
        var document = _service.GetById(documentID);

        if(document is not null)
        {
            _service.DeleteById(documentID);
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }
}
