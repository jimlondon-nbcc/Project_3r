using Project_3r.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_3r.Controllers
{
    public class StatesController : Controller
    {
        // GET: States
        /// <summary>
        ///     Creates view for All States (with sorting)
        /// </summary>
        /// <param name="sortBy">0 = StateCode, 1 = StateName</param>
        /// <param name="isDesc">false = Ascending Order, true = Descending Order</param>
        /// <returns>View</returns>
        [HttpGet]
        public ActionResult All(string id, int sortBy = 0, bool isDesc = false)
        {
            BooksEntities context = new BooksEntities();
            List<State> states = context.States.OrderBy(s => s.StateCode).ToList();

            switch (sortBy)
            {
                case 1:
                    {
                        if (isDesc)
                            states = context.States.OrderByDescending(s => s.StateName).ToList();
                        else
                            states = context.States.OrderBy(s => s.StateName).ToList();

                        break;
                    }
                case 0:
                default:
                    {
                        if (isDesc)
                            states = context.States.OrderByDescending(p => p.StateCode).ToList();
                        else
                            states = context.States.OrderBy(p => p.StateCode).ToList();

                        break;
                    }

            }
            //id is used as searchTerm 
            if (!string.IsNullOrWhiteSpace(id))
            {
                id = id.Trim().ToLower();


                states = states.Where(p =>
                p.StateCode.ToLower().Contains(id) ||
                p.StateName.ToLower().Contains(id)
                ).ToList();

            }
            states = states.Where(p => p.IsDeleted == false).ToList();
            return View(states);
        }
        /// <summary>          
        ///     Upsert GET method to fetch data 
        /// </summary>
        /// <param name="id">The StateCode of the state to update</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Upsert(string id)
        {
            BooksEntities context = new BooksEntities();
            State state = context.States.Where(p => p.StateCode == id).FirstOrDefault();
            if (state == null)
            {
                state = new State();
            }

            //Ensure deleted invoices aren't visible
            if (state.IsDeleted)
            {
                return RedirectToAction("All");
            }

            return View(state);
        }
        /// <summary>
        ///     Used to Update or Insert states
        /// </summary>
        /// <param name="state">The state being posted</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upsert(State state)
        {
            try
            {
                BooksEntities context = new BooksEntities();

                context.States.AddOrUpdate(state);

                context.SaveChanges();
            }
            catch (Exception) { }

            return RedirectToAction("All");
        }
        /// <summary>
        ///     HttpGet to delete a state
        /// </summary>
        /// <param name="id">The Id of the state to delete</param>
        /// <returns>Redirect to All States View</returns>
        [HttpGet]
        public ActionResult Delete(string id)
        {
            BooksEntities context = new BooksEntities();

            try
            {
                State state = context.States.Where(s => s.StateCode == id).FirstOrDefault();
                state.IsDeleted = true;

                context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToAction("All");
        }
    }
}