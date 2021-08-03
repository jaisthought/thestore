using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagazineStore
{
    class ResponseModel { }

    public class Token
    {
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }

    public class Category
    {
        public List<string> data { get; set; }
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }

    //Magazine
    public class Magazine
    {
        public List<MagazineData> data { get; set; }
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }

    public class MagazineData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
    }

    //Subscriber
    public class Subscriber
    {
        public List<SubscriberData> data { get; set; }
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }

    public class SubscriberData
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<int> magazineIds { get; set; }
    }

    //Answer
    public class AnswerBody
    {
        public List<string> subscribers { get; set; }
    }

    public class AnswerResponse
    {
        public AnswerResponseData data { get; set; }
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }

    public class AnswerResponseData
    {
        public string totalTime { get; set; }
        public bool answerCorrect { get; set; }
        public List<string> shouldBe { get; set; }
    }

}
