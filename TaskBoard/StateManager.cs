using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using TaskBoard.Models;

namespace TaskBoard
{
    public static class StateManager
    {
        private static string FILENAME = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\ViewState.xml";
        private static XmlDocument viewState;
        private static XmlNode root;
        static StateManager()
        {
            viewState = new XmlDocument();
            viewState.Load(FILENAME);
            root = viewState.GetElementsByTagName("Root")[0];
        }

        public static StickyNoteViewState GetState(int ID){

            foreach (XmlNode n in root.ChildNodes) {
                if (n.InnerText == ID.ToString()) { 
                    string i=null,j=null;
                    if (n.Attributes["X"] != null) { i = n.Attributes["X"].Value; };
                    if (n.Attributes["Y"] != null) { j = n.Attributes["Y"].Value; };
                    

                    return new StickyNoteViewState() { Transform = n.Attributes["Transform"].Value, X = i, Y = j };
                }
            }

            return null;
        }

        public static void SetState(int ID, string X, string Y, string Transform) 
        {

            foreach (XmlNode n in root.ChildNodes)
            {
                if (n.InnerText == ID.ToString())
                {
                    n.Attributes["X"].Value=X;
                    n.Attributes["Y"].Value=Y;
                    n.Attributes["Transform"].Value = Transform;
                    viewState.Save(FILENAME);
                    return;
                }
            }
            var node = viewState.CreateNode(XmlNodeType.Element, "StickyNote", "");
            node.InnerText=ID.ToString();
            
            var attx = viewState.CreateAttribute("X");
            attx.Value = X;

            var atty = viewState.CreateAttribute("Y");
            atty.Value = Y;

            var attt = viewState.CreateAttribute("Transform");
            attt.Value = Transform;

            node.Attributes.Append(attx);
            node.Attributes.Append(atty);
            node.Attributes.Append(attt);
            
            root.AppendChild(node);

            viewState.Save(FILENAME);
        }

        public static void ClearState(){
        
             while ( root.ChildNodes.Count>0){
                 root.RemoveChild(root.ChildNodes[0]);
             }
             viewState.Save(FILENAME);
        }
    
    }
}