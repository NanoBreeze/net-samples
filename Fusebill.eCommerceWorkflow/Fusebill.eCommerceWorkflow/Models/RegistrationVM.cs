﻿using Fusebill.ApiWrapper.Dto.Get;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fusebill.eCommerceWorkflow.Models
{
    public class RegistrationVM
    {
        //A list of all plans shown on the start page, currently contains bronze and gold plan
        public List<Plan> AvailablePlans { get; set; }
        
        //the ID of the plan that the user clicks on
        public long SelectedPlanId { get; set; }

        //A list of the available products in each plan, used in Step2
        public List<PlanProduct> AvailableProducts { get; set; }

        public List<long> AvailablePlanFrequencyIds { get; set; }

        public long SelectedPlanFrequencyID { get; set; }

        //How many of each product the user decides to have
        public Dictionary<string, decimal> QuantityOfProducts { get; set; }

        public Dictionary<string, bool> planProductIncludes { get; set; }


        //The Customer object contains properties pertaining to the customer, such as FrstName, LastName
        public Customer customerInformation { get; set; }

        //The Address objects contains properties pertaining to the billing address, such as City and PostalZip 
        public Address billingAddress { get; set; }

        public bool sameAsShipping { get; set; }

        public List<SelectListItem> listOfCountriesSLI {get;set;}

        public List<Country> listOfCountriesCountry { get; set; }

        public Customer postedCustomer { get; set; }

        //This has the planFrequencyID?
        public PlanOrderToCashCycle planOrderToCashCycle { get; set; }

        //The Subscription objects contains information needed for the customer to create a subscription
        public Subscription subscription { get; set; }

        //total cost of subscription
        public decimal subscriptionTotalCost { get; set; }

        //contains strongly typed keys for the session states
        public RegistrationStronglyTypedSessionState registrationStronglyTypedSessionState { get; set; }

        [Required(ErrorMessage="hi")]
        public string TestValidat { get; set; }

        public A asdf { get; set; }
    }
}