using FakeXrmEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using ProgresiaWorkflowActivity;
using System;
using System.Collections.Generic;

namespace ProgresiaUnitTest
{
    [TestClass]
    public class FindContactWithTwoParamsUnitTest
    {
        private XrmFakedContext fakedContext;
        private List<Guid> contactIds;

        [TestInitialize]
        public void Init()
        {
            fakedContext = new XrmFakedContext();

            var contact0 = new Entity("contact");
            contact0.Id = Guid.NewGuid();
            contact0["id"] = contact0.Id;
            contact0["name"] = "Contact_0";
            contact0["att1"] = string.Empty;
            contact0["att2"] = string.Empty;

            var contact1 = new Entity("contact");
            contact1.Id = Guid.NewGuid();
            contact1["id"] = contact1.Id;
            contact1["name"] = "Contact_1";
            contact1["att1"] = "Result_A";
            contact1["att2"] = "Info_A";

            var contact2 = new Entity("contact");
            contact2.Id = Guid.NewGuid();
            contact2["id"] = contact2.Id;
            contact2["name"] = "Contact_2";
            contact2["att1"] = "Result_A";
            contact2["att2"] = "Info_B";

            var contact3 = new Entity("contact");
            contact3.Id = Guid.NewGuid();
            contact3["id"] = contact3.Id;
            contact3["name"] = "Contact_3";
            contact3["att1"] = "Result_B";
            contact3["att2"] = "Info_B";

            var contact4 = new Entity("contact");
            contact4.Id = Guid.NewGuid();
            contact4["id"] = contact4.Id;
            contact4["name"] = "Contact_4";
            contact4["att1"] = "Result_B";
            contact4["att2"] = "Info_B";

            var contact5 = new Entity("contact");
            contact5.Id = Guid.NewGuid();
            contact5["id"] = contact5.Id;
            contact5["name"] = "Contact_5";
            contact5["att1"] = "Result_C";
            contact5["att2"] = "Info_C";

            fakedContext.Initialize(new List<Entity>()
            {
                contact0, contact1, contact2, contact3, contact4, contact5
            });

            contactIds = new List<Guid>()
            {
                contact0.Id, contact1.Id, contact2.Id, contact3.Id, contact4.Id, contact5.Id
            };
        }

        [TestMethod]
        public void DuplicatedAttributes()
        {
            var att = "att1";
            var param1 = "Result_A";
            var param2 = "Info_A";
            var inputs = new Dictionary<string, Object>() 
            { 
                { "FirstParam", param1 },
                { "FirstAttribute", att },
                { "SecondParam", param2 },
                { "SecondAttribute", att }
            };

            Assert.ThrowsException<ArgumentException>(() => fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs));
        }

        [TestMethod]
        public void DefaultParameters()
        {
            var att1 = "att1";
            var att2 = "att2";
            var inputs = new Dictionary<string, Object>() 
            {
                { "FirstAttribute", att1 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);
            var contactReference = ((EntityReference)result["ContactReference"]);
            
            Assert.IsTrue(contactReference.LogicalName.Equals("Contact_0"));
            Assert.IsTrue(contactReference.Id.Equals(contactIds[0]));
            Assert.IsTrue(((int)result["Status"]).Equals(1));
        }

        [TestMethod]
        public void NoContactFound()
        {
            var att1 = "att1";
            var att2 = "att2";
            var param1 = "Result_Z";
            var param2 = "Info_Z";
            var inputs = new Dictionary<string, Object>()
            {
                { "FirstParam", param1 },
                { "FirstAttribute", att1 },
                { "SecondParam", param2 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);

            Assert.IsNull(result["ContactReference"]);
            Assert.IsTrue(((int)result["Status"]).Equals(2));
        }

        [TestMethod]
        public void ContactFoundWithUniqueAttributes()
        {
            var att1 = "att1";
            var att2 = "att2";
            var param1 = "Result_A";
            var param2 = "Info_A";
            var inputs = new Dictionary<string, Object>()
            {
                { "FirstParam", param1 },
                { "FirstAttribute", att1 },
                { "SecondParam", param2 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);
            var contactReference = ((EntityReference)result["ContactReference"]);

            Assert.IsTrue(contactReference.LogicalName.Equals("Contact_1"));
            Assert.IsTrue(contactReference.Id.Equals(contactIds[1]));
            Assert.IsTrue(((int)result["Status"]).Equals(1));
        }

        [TestMethod]
        public void ContactFoundWithoutUniqueAttributes()
        {
            var att1 = "att1";
            var att2 = "att2";
            var param1 = "Result_A";
            var param2 = "Info_B";
            var inputs = new Dictionary<string, Object>()
            {
                { "FirstParam", param1 },
                { "FirstAttribute", att1 },
                { "SecondParam", param2 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);
            var contactReference = ((EntityReference)result["ContactReference"]);

            Assert.IsTrue(contactReference.LogicalName.Equals("Contact_2"));
            Assert.IsTrue(contactReference.Id.Equals(contactIds[2]));
            Assert.IsTrue(((int)result["Status"]).Equals(1));
        }

        [TestMethod]
        public void MoreThanOneContactFound()
        {
            var att1 = "att1";
            var att2 = "att2";
            var param1 = "Result_B";
            var param2 = "Info_B";
            var inputs = new Dictionary<string, Object>()
            {
                { "FirstParam", param1 },
                { "FirstAttribute", att1 },
                { "SecondParam", param2 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);

            Assert.IsNull(result["ContactReference"]);
            Assert.IsTrue(((int)result["Status"]).Equals(3));
        }

        [TestMethod]
        public void OnlyOneAttributeMatches()
        {
            var att1 = "att1";
            var att2 = "att2";
            var param1 = "Result_C";
            var param2 = "Info_Z";
            var inputs = new Dictionary<string, Object>()
            {
                { "FirstParam", param1 },
                { "FirstAttribute", att1 },
                { "SecondParam", param2 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);

            Assert.IsNull(result["ContactReference"]);
            Assert.IsTrue(((int)result["Status"]).Equals(2));
        }

        [TestMethod]
        public void TwoContactsHaveMatchesFromDifferentAttributes()
        {
            var att1 = "att1";
            var att2 = "att2";
            var param1 = "Result_C";
            var param2 = "Info_A";
            var inputs = new Dictionary<string, Object>()
            {
                { "FirstParam", param1 },
                { "FirstAttribute", att1 },
                { "SecondParam", param2 },
                { "SecondAttribute", att2 }
            };

            var result = fakedContext.ExecuteCodeActivity<FindContactWithTwoParams>(inputs);

            Assert.IsNull(result["ContactReference"]);
            Assert.IsTrue(((int)result["Status"]).Equals(2));
        }
    }
}
