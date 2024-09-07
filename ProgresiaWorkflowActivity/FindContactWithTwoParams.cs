using System;
using System.Activities;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;

namespace ProgresiaWorkflowActivity
{
    public class FindContactWithTwoParams : CodeActivity
    {
        [Input("First parameter")]
        public InArgument<string> FirstParam { get; set; }

        [RequiredArgument]
        [Input("First attribute")]
        public InArgument<string> FirstAttribute { get; set; }

        [Input("Second parameter")]
        public InArgument<string> SecondParam { get; set; }

        [RequiredArgument]
        [Input("Second attribute")]
        public InArgument<string> SecondAttribute { get; set; }

        [Output("Contact status")]
        public OutArgument<int> Status { get; set; }

        [Output("Contact reference")]
        [ReferenceTarget("contact")]
        public OutArgument<EntityReference> ContactReference { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string firstAttribute = FirstAttribute.Get(context);
            string secondAttribute = SecondAttribute.Get(context);

            if (firstAttribute == secondAttribute)
                throw new ArgumentException("Duplicated attributes.");

            string firstParam = FirstParam.Get(context) ?? string.Empty;
            string secondParam = SecondParam.Get(context) ?? string.Empty;

            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.InitiatingUserId);

            QueryByAttribute query = new QueryByAttribute("contact")
            {
                ColumnSet = new ColumnSet("name", "id")
            };

            query.AddAttributeValue(firstAttribute, firstParam);
            query.AddAttributeValue(secondAttribute, secondParam);

            EntityCollection results = service.RetrieveMultiple(query);
            
            var count = results.Entities.Count;

            if (count == 1)
            {
                Status.Set(context, 1);

                var entity = results.Entities.First();
                ContactReference.Set(context, new EntityReference(entity.GetAttributeValue<string>("name"), entity.GetAttributeValue<Guid>("id")));

                return;
            }

            if (count == 0)
            {
                Status.Set(context, 2);

                return;
            }

            Status.Set(context, 3);
        }
    }
}
