// Copyright 2020-2022 CYBERCRYPT

using EncryptonizeDBSample.Data;
using EncryptonizeDBSample.Models;
using Microsoft.EntityFrameworkCore;

namespace EncryptonizeDBSample.Services;

public class DocumentService
{
    private readonly StorageContext _context;

    public DocumentService(StorageContext context)
    {
        _context = context;
    }

    public Document? GetById(int documentID)
    {
        return _context.Documents
        .AsNoTracking()
        .SingleOrDefault(d => d.ID == documentID);
    }

    public Document CreateDocument(Document newDocument)
    {
        _context.Documents.Add(newDocument);
        _context.SaveChanges();

        return newDocument;
    }

    public void DeleteById(int documentID)
    {
        var documentToDelete = _context.Documents.Find(documentID);
        if (documentToDelete is not null)
        {
            _context.Documents.Remove(documentToDelete);
            _context.SaveChanges();
        }
    }
}
