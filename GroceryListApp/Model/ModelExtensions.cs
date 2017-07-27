using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace GroceryListApp.Model
{
    public static class ModelExtensions
    {
        public static Boolean AsBoolean(this string value)
        {
            return (value == null || value == "") ? false : (value == "yes" ? true : false);
        }

        public static async Task GetListItemsFromCollection<T>(ListView listView, string collection)
        {
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            Uri uri = new Uri("http://localhost:82/api/adata/data/" + collection  + "?xml=true");

            var response = await httpClient.GetStringAsync(uri);
            {
                if (response != null)
                {
                    List<T> groceries = new List<T>();
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response);
                    XmlNode root = doc.FirstChild;
                    if (root.HasChildNodes)
                    {
                        Type t = typeof(T);
                        FieldInfo[] fields = t.GetRuntimeFields().ToArray();

                        for (int i = 0; i < root.ChildNodes.Count; i++)
                        {
                            XmlNode node = root.ChildNodes[i];
                            if (node.NodeType == XmlNodeType.Element)
                            {
                                T obj = (T)System.Activator.CreateInstance(t);
                                for (int j = 0; j < node.ChildNodes.Count; j++)
                                {
                                    if (node.ChildNodes[j].FirstChild != null)
                                    {
                                        string name = node.ChildNodes[j].Name;
                                        FieldInfo finfo = fields.SingleOrDefault(f => f.Name.Substring(1, f.Name.IndexOf('>') - 1) == name);
                                        if (finfo != null)
                                        {
                                            string value = node.ChildNodes[j].FirstChild.Value;
                                            if (finfo.FieldType == typeof(string))
                                            {
                                                finfo.SetValue(obj, value);
                                            }
                                            else if (finfo.FieldType == typeof(Boolean))
                                            {
                                                finfo.SetValue(obj, Convert.ToBoolean(value));
                                            }
                                            else if (finfo.FieldType == typeof(double))
                                            {
                                                finfo.SetValue(obj, Convert.ToDouble(value));
                                            }
                                            else if (finfo.FieldType == typeof(int))
                                            {
                                                finfo.SetValue(obj, Convert.ToInt32(value));
                                            }
                                            else if (finfo.FieldType == typeof(DateTime))
                                            {
                                                finfo.SetValue(obj, Convert.ToDateTime(value));
                                            }
                                        }
                                    }
                                }
                                groceries.Add(obj);
                            }
                        }

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => listView.ItemsSource = groceries);
                    }
                }
            }
        }

        public static async Task PostItemToCollection<T>(ListView listView, string collection, object item)
        {
            await AddItemAndGetListItemsFromCollection<T>(listView, collection, item, false);
        }

        public static async Task AddItemAndGetListItemsFromCollection<T>(ListView listView, string collection, object item, bool getList)
        {
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            Uri uri = new Uri("http://localhost:82/api/adata/data/" + collection + "?xml=true");

            XmlDocument doc = new XmlDocument();
            // xmlns:http://api.blueduckconsultants.com/models
            XmlNode root = doc.CreateNode(XmlNodeType.Element, collection, "");

            Type t = typeof(T);
            FieldInfo[] fields = t.GetRuntimeFields().ToArray();

            foreach(FieldInfo f in fields)
            {
                object val = f.GetValue(item);
                if (val != null)
                {
                    XmlNode node = doc.CreateNode(XmlNodeType.Element, f.Name.Substring(1, f.Name.IndexOf('>')-1), "");
                    XmlNode data = doc.CreateTextNode(val.ToString());
                    node.AppendChild(data);
                    root.AppendChild(node);
                }
            }

            StringWriter sw = new StringWriter();
            XmlWriter xmloutput = XmlWriter.Create(sw);
            root.WriteTo(xmloutput);
            xmloutput.Flush();
            string xml = sw.ToString();

            System.Net.Http.HttpContent content = new System.Net.Http.StringContent(xml);
            content.Headers.ContentType.MediaType = "text/xml";
            var response = await httpClient.PostAsync(uri, content);
            {
                if (getList && response != null)
                {
                    await GetListItemsFromCollection<T>(listView, collection);
                }
            }
        }
    }

}
