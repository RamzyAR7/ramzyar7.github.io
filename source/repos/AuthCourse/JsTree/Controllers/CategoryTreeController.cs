using JsTree.Data;
using JsTree.Models;
using JsTree.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace JsTree.Controllers
{
    public class CategoryTreeController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CategoryTreeController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            var categories = _applicationDbContext.Categories.Include(c => c.Courses).ToList();


            List<TreeViewNode> nodes = new List<TreeViewNode>();

            int index = 1;
            int index2 = 1;

            foreach (var category in categories)
            {
                nodes.Add(new TreeViewNode(index.ToString(), "#", category.Name));
                foreach (var course in category.Courses)
                {
                    nodes.Add(new TreeViewNode($"{index.ToString()}_{index2.ToString()}", course.CategoryId.ToString(), course.Name));
                    index2++;
                }
                index++;
            }
            ViewBag.JsonTree = JsonConvert.SerializeObject(nodes);
            return View();
        }
        [HttpPost]
        public IActionResult Index(string SelectedItems)
        {
            List<TreeViewNode> treeViewNodes = JsonConvert.DeserializeObject<List<TreeViewNode>>(SelectedItems);

            return RedirectToAction("Index");
        }
    }
}
