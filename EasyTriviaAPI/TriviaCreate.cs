using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EasyTriviaAPI
{
  public class TriviaCreate
  {
    public string Username { get; set; }
    public string TriviaQuestion { get; set; }
  }

  public class TriviaCheck
  {
    public string Username { get; set; }
  }
  public class TriviaResponse
  {
    [BsonElement("_id")]
    public ObjectId objId { get; set; }
    public string Username { get; set; }
    public string TriviaQuestion { get; set; }
  }

  public class TriviaResult
  {
    public List<TriviaResponse> triviaList { get; set; }
  }

}