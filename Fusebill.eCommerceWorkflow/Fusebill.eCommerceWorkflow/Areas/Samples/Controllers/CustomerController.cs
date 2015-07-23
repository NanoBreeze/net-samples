﻿using Fusebill.eCommerceWorkflow.Areas.Samples.Models;
using Fusebill.eCommerceWorkflow.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fusebill.eCommerceWorkflow.Areas.Samples.Controllers
{
    public class CustomerController : FusebillBaseController
    {
        //
        // GET: /Samples/Customer/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateCustomer()
        {
            var postCustomer = new Fusebill.ApiWrapper.Dto.Post.Customer
            {
                FirstName = "Jack",
                LastName = "Purwell",
                PrimaryEmail = "J.Purwell@example.com",
                PrimaryPhone = "1234567890",
            };

            var returnedCustomer = ApiClient.PostCustomer(postCustomer);

            return Json(returnedCustomer);
        }

        [HttpPost]
        /// <summary>
        /// Cancel newly created customer
        /// </summary>
        public void CancelCustomer(CustomerCancellationVM customerCancellation)
        {
            ApiClient.PostCustomerCancel(new ApiWrapper.Dto.Post.CustomerCancel
            {
                CustomerId = customerCancellation.id,
                CancellationOption = customerCancellation.cancellationOption
            }
            );
        }

    }
}
