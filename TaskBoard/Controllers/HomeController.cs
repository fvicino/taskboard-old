using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Collections;
using TaskBoard.Models;
using System.Configuration;

namespace TaskBoard.Controllers
{
    public class HomeController : Controller
    {
        const string TFSURL = "http://ap-syd-tfs:8080/tfs/mojo";
        const string WORKITEMEDITURL = "http://ap-syd-tfs/sites/Mojo/_layouts/tswa/UI/Pages/WorkItems/WorkItemEdit.aspx";
        const string WORKITEMEDITPARAMSTRING = "?id={0}";

        public ActionResult Index()
        {
            List<TaskStickyNote> notes = new List<TaskStickyNote>();

                //connect to tfs server
                // The url to the tfs server 
                Uri tfsUri = new Uri(TFSURL);

                // Load the tfs project collection 
                var mojo = new TfsTeamProjectCollection(tfsUri);

                // Log in to TFS 
                mojo.EnsureAuthenticated();

                var wit = mojo.GetService<WorkItemStore>();

                Guid qguid = new Guid(ConfigurationManager.AppSettings["QueryGUID"]);
                var queryDefinition = wit.GetQueryDefinition(qguid);

                string wiql = queryDefinition.QueryText;

                System.Collections.Hashtable context = new System.Collections.Hashtable();

                context.Add("project", queryDefinition.Project.Name);

                var queryReturn = new Query(wit, wiql, context);
                var wic = queryReturn.RunQuery(); //.RunLinkQuery();
                
                foreach (WorkItem wi in wic)
                {
                    TaskStickyNote sn = new TaskStickyNote(wi, StateManager.GetState(wi.Id));
                    sn.EditURL = WORKITEMEDITURL + String.Format(WORKITEMEDITPARAMSTRING, wi.Id.ToString());
                    notes.Add(sn);
                }

                notes = notes.OrderBy(x => (x.RemainingTime == "0" && x.AssignedTo == String.Empty ? "" : x.RemainingTime)).OrderBy(x => x.AssignedTo).OrderBy(x => x.State).ToList();

                return View(notes);

        }

        public ActionResult About()
        {
            return View();
        }

        public void SaveViewState(int ID, string X, string Y, string Transform)
        {
            StateManager.SetState(ID,X,Y,Transform);
        }

        public ActionResult ClearState() { 
            StateManager.ClearState();
            return RedirectToAction("Index");
        }

    }
}
