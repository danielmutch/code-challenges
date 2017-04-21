﻿using System;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace DocDb.Tweets
{
    public class DocumentDbService
    {
        private DocumentClient _client;
        private Uri _collectionUri;

        public async Task Initalise()
        {
            var endpoint = CloudConfigurationManager.GetSetting("DocumentDB:Endpoint");
            var key = CloudConfigurationManager.GetSetting("DocumentDB:Key");

            _client = new DocumentClient(new Uri(endpoint), key);

            var database = CloudConfigurationManager.GetSetting("DocumentDB:DatabaseName");
            var collection = CloudConfigurationManager.GetSetting("DocumentDB:CollectionName");
            _collectionUri = UriFactory.CreateDocumentCollectionUri(database, collection);

            try
            {
                await _client.CreateDatabaseAsync(new Database {Id = database});
                await _client.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(database), new DocumentCollection {Id = collection});
            }
            catch (DocumentClientException)
            {
                //Ignore - It probably already exists
            }

            await _client.ReadDocumentCollectionAsync(_collectionUri);
        }

        public async Task UploadDocument(object document)
        {
            await _client.CreateDocumentAsync(_collectionUri, document);
        }
    }
}