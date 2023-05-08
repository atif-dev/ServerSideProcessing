using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServerSideProcessing.Models;
using System.Linq.Dynamic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Web.Helpers;
using System.IO;

namespace ServerSideProcessing.Controllers
{
    public class EmployeeController : Controller
    {
        
        //
        // GET: /Employee/
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult ViewAll()
		{
			return View(GetAllEmployee());
		}

		IEnumerable<Employee> GetAllEmployee()
		{
			using (DBModel1 db = new DBModel1())
			{
				return db.Employees.ToList<Employee>();
			}

		}

		[HttpPost]
        public ActionResult GetList()
        {
            //Server Side Parameter
            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            List<Employee> empList = new List<Employee>();
            using (DBModel1 db = new DBModel1())
            {
                empList = db.Employees.ToList<Employee>();
                int totalrows = empList.Count;
                if (!string.IsNullOrEmpty(searchValue))//filter
                {
                    empList = empList. 
                        Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) || x.Position.ToLower().Contains(searchValue.ToLower()) || x.Office.ToLower().Contains(searchValue.ToLower()) || x.Age.ToString().Contains(searchValue.ToLower()) || x.Salary.ToString().Contains(searchValue.ToLower())).ToList<Employee>();
                }
                int totalrowsafterfiltering = empList.Count;
                //sorting
                empList = empList.OrderBy(sortColumnName + " " + sortDirection).ToList<Employee>();

                //paging
                empList = empList.Skip(start).Take(length).ToList<Employee>();


                return Json(new { data = empList, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfiltering }, JsonRequestBehavior.AllowGet);
            }

           
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Employee());
            else
            {
                using (DBModel1 db = new DBModel1())
                {
                    return View(db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>());
                    
                }
            }
        }

		public ActionResult Add(int id = 0)
		{
			Employee emp = new Employee();
			if (id != 0)
			{
				using (DBModel1 db = new DBModel1())
				{
					emp = db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
				}
			}
			return View(emp);
		}

		[HttpPost]
		public ActionResult Add(Employee emp)
		{
			try
			{
				using (DBModel1 db = new DBModel1())
				{
					if (emp.EmployeeID == 0)
					{
						db.Employees.Add(emp);
						db.SaveChanges();
					}
					else
					{
						db.Entry(emp).State = EntityState.Modified;
						db.SaveChanges();

					}		
				}
				return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{

				return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
			}
		}

		/*[HttpGet]
		public ActionResult Add(int id = 0)
		{
			if (id == 0)
				return View(new Employee());
			else
			{
				using (DBModel1 db = new DBModel1())
				{
					return View(db.Employees.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>());
				}
			}
		}

		[HttpPost]
		public ActionResult Add(Employee emp)
		{
			using (DBModel1 db = new DBModel1())
			{
				if (emp.EmployeeID == 0)
				{
					db.Employees.Add(emp);
					db.SaveChanges();
					return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
				}
				else
				{
					db.Entry(emp).State = EntityState.Modified;
					db.SaveChanges();
					return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
				}
			}


		}*/



		/* [HttpPost]
         public ActionResult AddOrEdit(Employee emp)
         {
             using (DBModel1 db = new DBModel1())
             {
                 if (emp.EmployeeID == 0)
                 {
                     db.Employees.Add(emp);
                     db.SaveChanges();
                     return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                 }
                 else
                 {
                     db.Entry(emp).State = EntityState.Modified;
                     db.SaveChanges();
                     return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                 }
             }


         }*/
	}
}