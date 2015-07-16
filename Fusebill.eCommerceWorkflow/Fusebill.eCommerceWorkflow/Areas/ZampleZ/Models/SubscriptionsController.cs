﻿using Fusebill.ApiWrapper;
using Fusebill.eCommerceWorkflow.Areas.ZampleZ.Models;
using Fusebill.eCommerceWorkflow.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Fusebill.eCommerceWorkflow.Areas.ZampleZ.Controllers
{
    //public class House
    //{
    //    public int Number { get; set; }
    //    public List<int> Numbers { get; set; }

    //    public int[] NumbersArray { get; set; }

    //    public string Name { get; set; }
    //}

    //after a subscription is deleted, refresh the screen to show the updated  subscriptions 
    //cancel options, more options
    //same with reverse charges
    //subscription provision with activatio month and date options; refresh screen
    //subscription cancel with activation cancel options; refresh screen
    //there seems to be an issue with the activation timestamp with products quprice overrices (cannot override something with a scheduled activation timestamp)
    //adding label in one line
    //call a function to create labels


    public class SubscriptionsController : FusebillBaseController
    {
        //
        // GET: /ZampleZ/Subscriptions/

        //public void PassHouses(House house)
        //{
        //    var t = house;
        //}

        public ActionResult Index()
        {
            var demoCustomerIds = ConfigurationManager.AppSettings["DemoCustomerIds"].Split(',');

            var customersAndSubscriptionsVM = new CustomersAndSubscriptionsVM
            {
                AvailableCustomers = new List<ApiWrapper.Dto.Get.Customer>(),
                AvailablePlans = new List<ApiWrapper.Dto.Get.Plan>()
            };

            //add available customers
            foreach (var customerId in demoCustomerIds)
            {
                customersAndSubscriptionsVM.AvailableCustomers.Add(ApiClient.GetCustomer(Convert.ToInt64(customerId)));
            }

            //add availalable subscriptions
            var availablePlanIds = ConfigurationManager.AppSettings["DesiredPlanIds"];
            var availablePlans = availablePlanIds.Split(',');

            foreach (var plan in availablePlans)
            {
                customersAndSubscriptionsVM.AvailablePlans.Add(ApiClient.GetPlan(Convert.ToInt64(plan)));
            }

            return View(customersAndSubscriptionsVM);
        }

        public void CancelCustomer(PostCustomerIdVM postCustomerIdVM)
        {
            ApiClient.PostCustomerCancel(new ApiWrapper.Dto.Post.CustomerCancel
            {
                CustomerId = postCustomerIdVM.CustomerID,
                CancellationOption = "Full"
            });
        }

        public void ReverseCharge(PostCustomerIdVM fillerInteger)
        {
            var a = ApiClient.GetInvoicesByCustomerId(fillerInteger.CustomerID, new QueryOptions());
            var x = a.Results[0].Charges[0].Id;

            // ApiClient.PostReverseCharge(new ApiWrapper.Dto.Post.ReverseCharge{})

            ApiClient.PostReverseCharge(new ApiWrapper.Dto.Post.ReverseCharge
            {
                // ReverseChargeOption = "Full",
                ReverseChargeAmount = 1.05M,
                Reference = "Hello",
                ChargeId = x
            });

            var asdf = 4;


        }

        [HttpPost]
        public ActionResult ListSubscriptionsForCustomer(PostCustomerIdVM postCustomerIdVM)
        {

            long desiredCustomerID = Convert.ToInt64(postCustomerIdVM.CustomerID);

            var subscriptions = ApiClient.GetSubscriptions(desiredCustomerID, new Fusebill.ApiWrapper.QueryOptions()).Results;
           


            return Json(subscriptions);
        }


        [HttpPost]
        public void UpdateSubscription(PostSubscriptionVM postSubscriptionVM)
        {

            var subscription = ApiClient.GetSubscription(postSubscriptionVM.SubscriptionID);

            for (int i = 0; i < subscription.SubscriptionProducts.Count; i++)
            {
                //Editing the plan product's quantit             
                subscription.SubscriptionProducts[i].Quantity = postSubscriptionVM.ProductQuantityOverrides[i];

                //editing the plan product's price
                subscription.SubscriptionProducts[i].SubscriptionProductPriceOverride = new ApiWrapper.Dto.Get.SubscriptionProductPriceOverride
                {
                    ChargeAmount = postSubscriptionVM.ProductPriceOverrides[i]
                };

                ////Editing a non-mandatory plan product to be included
                // var inclusion = session.AvailableProducts[i].IsIncluded;
                // subscription.SubscriptionProducts[i].IsIncluded = inclusion;
            }

            //Editing a subscription's name, description, charge, setupFee, and ID  NOTE: To verify, uri is not included in the dto but is in the help.fusebill.com documentation. DOes API help mean "Sample" when it writes "Simple"?

            subscription.SubscriptionOverride = new ApiWrapper.Dto.Get.SubscriptionOverride
            {
                Name = postSubscriptionVM.NameOverride,
                Description = postSubscriptionVM.DescriptionOverride,
                Charge = postSubscriptionVM.ChargeOverride,
                SetupFee = postSubscriptionVM.SetupOverride,
                Id = postSubscriptionVM.SubscriptionID
            };



            //Editing the reference
            subscription.Reference = postSubscriptionVM.Reference;

            //Editing Contract Start and End Dates

            subscription.ContractStartTimestamp = postSubscriptionVM.ContractStartTimestamp;
            subscription.ContractEndTimestamp = postSubscriptionVM.ContractEndTimestamp;

            //Editing the Scheduled Activation Date

            //subscription.ScheduledActivationTimestamp = postSubscriptionVM.ScheduledActivationTimestamp;

            //Editing the expiration period NOTE: Typo : "This can SET BY submitting..."
            subscription.RemainingInterval = postSubscriptionVM.RemainingInterval; //setting the RemainingInterval property to 0 will result in an initial charge and then an immediate expiry of the subscription following activation.


            Automapping.SetupSubscriptionGetToPutMapping();
            var putSubscription = AutoMapper.Mapper.Map<Fusebill.ApiWrapper.Dto.Get.Subscription, Fusebill.ApiWrapper.Dto.Put.Subscription>(subscription);

            ApiClient.PutSubscription(putSubscription);
            int a = 4;


        }

        //[HttpPost] Include this later
        /// <summary>
        /// To create/post a subscription, we must specify which customer and which plan the subscription is for by using the customerID and planFrequencyID fields.
        /// </summary>
        /// <returns>ApiWrapper.Dto.Get.Subscription</returns>
        public void CreateSubscription(CreateSubscriptionVM createSubscriptionVM)
        {

            var postSubscription = new ApiWrapper.Dto.Post.Subscription
            {
                CustomerId = Convert.ToInt64(createSubscriptionVM.CustomerID),
                PlanFrequencyId = createSubscriptionVM.PlanFrequencyID
            };

            ApiClient.PostSubscription(postSubscription);


            var a = 4;
        }


        [HttpPost]
        /// <summary>
        /// For this demo, we first create a subscription and then delete it.
        /// </summary>
        /// <returns></returns>
        public void DeleteSubscription(PostSubscriptionIdVM postSubscriptionIdVM)
        {

            ApiClient.DeleteSubscription(postSubscriptionIdVM.SubscriptionID);

            var a = 6;
        }

        [HttpPost]
        /// <summary>
        /// For this demo, we first create a subscription and then delete it.
        /// </summary>
        /// <returns></returns>
        public void ProvisionSubscription(PostSubscriptionIdVM postSubscriptionIdVM)
        {

            var postSubscriptionProvision = new Fusebill.ApiWrapper.Dto.Post.SubscriptionProvision
            {
                Id = postSubscriptionIdVM.SubscriptionID,
                InvoiceDay = 11,
                InvoiceMonth = 9
            };

            ApiClient.PostSubscriptionProvision(postSubscriptionProvision);

            var a = 6;
        }

        public void ActivateSubscription(PostSubscriptionIdVM postSubscriptionIdVM)
        {

            var postSubscriptionActivation = new Fusebill.ApiWrapper.Dto.Post.SubscriptionActivation
            {
                Id = postSubscriptionIdVM.SubscriptionID,
                InvoiceDay = 11,
                InvoiceMonth = 9
            };

            ApiClient.PostSubscriptionActivation(postSubscriptionActivation);

            var a = 6;
        }


        public void CancelSubscription(PostSubscriptionIdVM postSubscriptionIdVM)
        {

            var postSubscriptionCancel = new Fusebill.ApiWrapper.Dto.Post.SubscriptionCancel
            {
                SubscriptionId = postSubscriptionIdVM.SubscriptionID,
                CancellationOption = "Full"
            };

            ApiClient.PostSubscriptionCancel(postSubscriptionCancel);

            var a = 6;
        }


        //.  I think API meant to use "GetCustomer" instead of "GetCustomers". "GetCustomer" doesn't have the specified properties and neither does GetCustomers
        /// <summary>
        /// This action will return an array that contains a list of Subscriptions  applied against the specified Customer Id. 
        /// This call will return all Subscriptions regardless of status and will return an empty array if the Customer specified has no Subscriptions.
        /// </summary>
        /// <returns></returns>

        //The PostSubscriptionProvision takes an object, not a id
        /// <summary>
        /// 
        /// </summary>
        /// <returns>ApiWrapper.Dto.Get.Subscription</returns>
        public ActionResult Provision()
        {
            Fusebill.ApiWrapper.Dto.Post.SubscriptionProvision subscriptionProvision = new ApiWrapper.Dto.Post.SubscriptionProvision();
            subscriptionProvision.Id = Convert.ToInt64(ConfigurationManager.AppSettings["SubscriptionDemoSubscriptionID"]);
            subscriptionProvision.InvoiceDay = 15;
            subscriptionProvision.InvoiceMonth = 7;
            //billing period ID?

            ApiClient.PostSubscriptionProvision(subscriptionProvision);

            return View();
        }

        //POST SUBSCRIPTIONACTIVATIONEMAIL???? Where is postActivation
        /// <summary>
        /// This action will set the status of a Subscription to Active, which in most cases this will generate an invoice, and depending on the customer's billing options, apply a charge and a collection attempt against the Customer's payment method.
        /// Alternatively, you can call this call and pass in the "preview" field. 
        /// If "preview" is set to True then this will mimic the actual activation process and return a mock invoice which details the total value of this subscription. 
        /// You can then perform a POST /v1/Payments/ and if the payment is  successful you can then call this again without the "preview" field which actually activates the Subscription.
        /// </summary>
        /// <returns>ApiWrapper.Dto.Get.Subscription</returns>
        public ActionResult Activation()
        {
            Fusebill.ApiWrapper.Dto.Post.SubscriptionActivation subscriptionActivation = new ApiWrapper.Dto.Post.SubscriptionActivation();
            subscriptionActivation.Id = Convert.ToInt64(ConfigurationManager.AppSettings["SubscriptionDemoSubscriptionID"]);
            subscriptionActivation.InvoiceDay = 15;
            subscriptionActivation.InvoiceMonth = 7;
            //billing period ID?


            ApiClient.PostSubscriptionActivation(subscriptionActivation, preview: true);
            return View();
        }

        //I'm assuming that SUbscriptionCancellation is the same as SubscriptionCancel. Where the heck is the cancel method in the client.cs class???
        /// <summary>
        /// This action cancels an active Subscription, (only Subscriptions in Active status can be cancelled.)
        /// </summary>
        /// <returns>ApiWrapper.Dto.Get.Subscription</returns>
        public ActionResult Cancellation()
        {
            Fusebill.ApiWrapper.Dto.Post.SubscriptionCancel subscriptionCancel = new ApiWrapper.Dto.Post.SubscriptionCancel();
            subscriptionCancel.SubscriptionId = Convert.ToInt64(ConfigurationManager.AppSettings["SubscriptionDemoSubscriptionID"]);

            /*This field defines if the Customer receives a refund when the Subscription is canceled. Valid string are "None", "Unearned", "Full". 
             * None indicates the Customer will not receive a refund. 
             * Unearned indicates the customer will receive the Unearned revenue or Prorated Amount. 
             * Full indicates the customer will receive a full refund for the current billing period. 
             * */
            subscriptionCancel.CancellationOption = "Full";

            ApiClient.PostSubscriptionCancel(subscriptionCancel);
            return View();
        }

        //Cannot locate APiClient.postSubscriptionProductPriceOverride QQ
        /// <summary>
        /// This action creates a price override on a Subscription Product. 
        /// The override modifies the price which will be charge from that point forward when the product charges or quantity increases and is purchased.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreatePriceOverride()
        {

            return View();
        }

        //Same issue as above
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult EditPriceOverride()
        {

            return View();
        }

        //Uhhh, where is the delete folder in the core???
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult DeletePriceOverride()
        {
            return View();
        }

    }
}