﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DMS.Application.DTOs;
using DMS.Application.DTOs.Documents;
using DMS.Domain.Abstract;
using DMS.Domain.Entities;

namespace DMS.Application.Documents
{
  public class AppDocumentService : IAppDocumentService
  {
    private readonly IRepository<Document> docRepo;
    private readonly IRepository<AppUser> userRepo;
    private readonly IMapper mapper;

    public AppDocumentService(IRepository<Document> docRepo, IRepository<AppUser> userRepo, IMapper mapper)
    {
      this.docRepo = docRepo;
      this.userRepo = userRepo;
      this.mapper = mapper;
    }

    public async Task<int> CreateDocument(DocumentContentsDto d)
    {
      var author = await userRepo.GetById(d.AuthorId);
      var document = new Document(d.Title, d.Body, author);
      await docRepo.Create(document);
      return document.Id;
    }

    public async Task<int> EditDocument(DocumentContentsDto d)
    {
      try
      {
        var editor = await userRepo.GetById(d.AuthorId);
        var document = await docRepo.GetById(d.Id);
        document.Edit(editor, d.Title, d.Body);
        await docRepo.Update(document);
      }catch(Exception e)
      {
        // Log
      }
      return d.Id;
    }

    public async Task<DocumentWithHistoryDto> GetFullDocument(int documentId, int userId)
    {
      var document = await docRepo.GetById(documentId);
      var user = await userRepo.GetById(userId);
      var dto = mapper.Map<DocumentWithHistoryDto>(document);
      dto.AvailableStatusChanges = document?.AvailableStatusChanges(user);
      return dto;
    }

    public async Task<DocumentContentsDto> GetDocumentContents(int id)
    {
      var document = await docRepo.GetById(id);
      var dto = mapper.Map<DocumentContentsDto>(document);
      return dto;
    }

    public async Task<bool> ChangeStatus(int documentId, int appUserId, DocumentStatus status, string message)
    {      
      try
      {
        var document = await docRepo.GetById(documentId);
        var user = await userRepo.GetById(appUserId);

        document.ChangeStatus(user, status, message);
        await docRepo.Update(document);
      }
      catch (Exception e)
      {
        return false;
      }
      return true;
    }

    public IEnumerable<DocumentSummaryDto> FindDocuments(Func<Document, bool> predicate)
    {
      var documents = docRepo.GetAll().Where(predicate);
      var summaries = mapper.Map<IEnumerable<DocumentSummaryDto>>(documents);
      return summaries;
    }   
  }
}
