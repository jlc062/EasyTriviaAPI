  using System;
  using System.IO;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Azure.WebJobs;
  using Microsoft.Azure.WebJobs.Extensions.Http;
  using Microsoft.AspNetCore.Http;
  using Microsoft.Extensions.Logging;
  using Newtonsoft.Json;
  using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Collections.Generic;

namespace EasyTriviaAPI
  {
      public static class EasyTriviaAPI
      {
          [FunctionName("EasyTriviaAPI")]
          public static async Task<IActionResult> Run(
              [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
              ILogger log)
          {

            IMongoCollection<BsonDocument> collection = CreateDBCollection();

            if (req.Method == "POST")
            {
              string result = new StreamReader(req.Body).ReadToEnd();
              TriviaCreate triviaCreate = JsonConvert.DeserializeObject<TriviaCreate>(result);
              

              CreateQuestion(triviaCreate, collection);
              string responseMessageGood = "ok";

              return new OkObjectResult(responseMessageGood);
      }
              if (req.Method == "GET")
              {
                string result = new StreamReader(req.Body).ReadToEnd();
                TriviaCheck triviaCheck = JsonConvert.DeserializeObject<TriviaCheck>(result);
                string username = triviaCheck.Username;

                List<BsonDocument> triviaDocumentList = CheckTrivia(collection, username);
                TriviaResult triviaResult = new TriviaResult();
                triviaResult.triviaList = new List<TriviaResponse>();
                foreach (var document in triviaDocumentList)
                {
                  TriviaResponse triviaResponse = BsonSerializer.Deserialize<TriviaResponse>(document);
                  triviaResult.triviaList.Add(triviaResponse);
                }

                var Json = JsonConvert.SerializeObject(triviaResult);

                return new OkObjectResult(Json);
              }
              string responseMessage = "bad request";

              return new OkObjectResult(responseMessage);


    }

    private static List<BsonDocument> CheckTrivia(IMongoCollection<BsonDocument> collection, string username)
    {
      var filter = Builders<BsonDocument>.Filter.Eq("Username", username);
      var triviaDocumentList = collection.Find(filter).ToList();
      return triviaDocumentList;
    }

    private static IMongoCollection<BsonDocument> CreateDBCollection()
    {
      MongoClient mongoClient = new MongoClient(Environment.GetEnvironmentVariable("ConnectionString"));
      var dataBase = mongoClient.GetDatabase("trivia");
      var collection = dataBase.GetCollection<BsonDocument>("trivias");
      return collection;
    }

    private static void CreateQuestion(TriviaCreate triviaCreate, IMongoCollection<BsonDocument> collection)
    {
      var document = new BsonDocument {
                          { "Username", triviaCreate.Username },
                          { "TriviaQuestion", triviaCreate.TriviaQuestion }
                        };

      collection.InsertOne(document);
    }
  }
  }

