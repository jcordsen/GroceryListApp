using System;
using System.Collections.Generic;
using System.Text;

namespace GroceryListApp.Model
{
    /*
    {
        "_id":"65c52089-f74a-59d1-4eca-f8715d9edecb",
        "image":"",
        "imageType":"none",
        "ownerId":"593b26910822f34308860cba",
        "what":"Apple"
    }
    */

    public class GroceryItem
    {
        public string _id { get; set; }
        public string image { get; set; }
        public string imageType { get; set; }
        public string ownerId { get; set; }
        public string what { get; set; }
        public string bought { get; set; }
    }
}
