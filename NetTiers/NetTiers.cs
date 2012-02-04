/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
 using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Library.Common.Database
{
    /// <summary>
    /// Abstraction for Inserting and Updating records through the NetTiers interface.
    /// </summary>
    public static class NetTiers
    {

        /// <summary>
        /// Checks if the entity is valid against the Validation Rules in netTiers
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="page">The page.</param>
        /// <param name="label">The label to display the errors.</param>
        /// <returns>Passing validation, the object will be returned, else returns null.</returns>
        public static object DisplayIsValid(object entity, Page page, Label label)//Change object to Entity
        {
            return !NetTiers.IsValid(entity, page, label) ? null : entity;
        }

        /// <summary>
        /// Determines whether the specified item is valid.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="page">The page.</param>
        /// <param name="lbl">The label.</param>
        /// <returns>
        /// 	<c>true</c> if the specified item is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(object item, Page page = null, Label lbl = null)
        {
            if (item == null) return false;

            if(page == null) page = new Page();
            if(lbl == null) lbl = new Label();

            bool isValid = DoValidCheck(item);

            if (!isValid)
            {
                string message = "";
                string alert = "";

                foreach (object rule in ( (IEnumerable) item.GetType().GetProperty("BrokenRulesList", new Type[] { }).GetValue(item, new Type[] { }) ))
                {
                    message = AddMessage(message, rule);
                    alert = AddAlert(item, rule, alert);
                }

                SetMessageToLabel(lbl, message);
                AddAlertToPage(page, alert);
            }

            return isValid;
        }

        /// <summary>
        /// Does the valid check.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        private static bool DoValidCheck(object item)
        {
            return (bool)item.GetType().GetProperty("IsValid").GetValue(item, new Type[] { });
        }
        /// <summary>
        /// Adds the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        private static string AddMessage(string message, object rule)
        {
            message += string.Format("<li>{0} - {1}</li>", rule.GetType().GetProperty("Property").GetValue(rule, new Type[] { }), rule.GetType().GetProperty("Description").GetValue(rule, new Type[] { }));
            return message;
        }
        /// <summary>
        /// Adds the alert.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="rule">The rule.</param>
        /// <param name="alert">The alert.</param>
        /// <returns></returns>
        private static string AddAlert(object item, object rule, string alert)
        {
            alert += string.Format("*" + item.GetType().Name + ".{0}\\r\\n", rule.GetType().GetProperty("Description").GetValue(rule, new Type[] { }));
            return alert;
        }
        /// <summary>
        /// Sets the message to label.
        /// </summary>
        /// <param name="lbl">The LBL.</param>
        /// <param name="message">The message.</param>
        private static void SetMessageToLabel(Label lbl, string message)
        {
            lbl.Text += "<BR>" + message;
        }
        /// <summary>
        /// Adds the alert to page.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="alert">The alert.</param>
        private static void AddAlertToPage(Page page, string alert)
        {
            ScriptManager.RegisterClientScriptBlock(page, page.GetType(), page.ID + "_NetTiersValidationScript", "alert('" + alert + "');", true);
        }




        /// <summary>
        /// Entiry Properties that are common to all. These are common to all databse tables.
        /// </summary>
        /// <remarks>
        /// I have them here to remind me what they are.
        /// </remarks>
        private struct Property
        {
            public const string Uid = "Uid",
                                CreateDate = "CreateDate",
                                UpdateDate = "UpdateDate",
                                DeleteDate = "DeleteDate";
        }

        /// <summary>
        /// Actions to be taken by Record
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Insert into the database
            /// </summary>
            Insert = 0,
            /// <summary>
            /// Update the entity in the databse
            /// </summary>
            Update = 1,
            /// <summary>
            /// Add DeleteDate into the database
            /// </summary>
            Delete = 2,
            /// <summary>
            /// Nulls the DeleteDate in the database
            /// </summary>
            UnDelete = 3
        }

        /// <summary>
        /// Generates the entity data.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>XML string of the entities columns and data</returns>
        private static string GenerateEntityData(object entity)
        {
            string xml = "";
            string tableName = entity.GetType().GetProperty("TableName").GetValue(entity, null).ToString();

            xml += "<" + tableName + ">";
            xml = Enum.GetNames(Type.GetType(entity.GetType() + "Column" + ", " + entity.GetType().Namespace))
                .Cast<object>().Aggregate(xml, (current, str) => current + ( "<" + str + ">" + "<![CDATA[" + entity.GetType().GetProperty(str.ToString()).GetValue(entity, null) + "]]>" + "</" + str + ">" ));
            xml += "</" + tableName + ">";

            return xml;
        }

        /// <summary>
        /// Creates the service.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>New Service</returns>
        private static object CreateService(object entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            string serviceNamespace = entity.GetType().Namespace.Replace("Entities", "Services");
            Assembly asm = Assembly.LoadFrom(Universal.MappedApplicationPath() + @"\bin\" + serviceNamespace + ".dll");
            Type t = asm.GetType(serviceNamespace + "." + entity.GetType().Name + "Service");
            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// Records the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="action">The action.</param>
        /// <param name="service">The service.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="utcOffset">The UTC offset.</param>
        public static void Record(object entity, Action action = Action.Insert, object service = null, int userId = -1, double utcOffset = 0)
        {
            if (entity == null) throw new ArgumentNullException("entity");

            if (service == null) service = CreateService(entity);

            DateTime dtCurrent = DateTime.UtcNow.AddHours(utcOffset);

            //All Actions update 'UpdateDate'
            entity.GetType().GetProperty(Property.UpdateDate).SetValue(entity, dtCurrent, null);

            switch (action)
            {
                case Action.Update:
                    /*
                     * Nothing needs to happen here because we set the UpdateDate above
                     */
                    break;

                case Action.Insert:
                    entity.GetType().GetProperty(Property.CreateDate).SetValue(entity, dtCurrent, null);
                    break;

                case Action.Delete:
                    entity.GetType().GetProperty(Property.DeleteDate).SetValue(entity, dtCurrent, null);
                    break;

                case Action.UnDelete:
                    entity.GetType().GetProperty(Property.DeleteDate).SetValue(entity, null, null);
                    break;

                default:
                    throw new ArgumentException("action");
            }
            try
            {
                service.GetType()
                    .GetMethod("Save", new[] { entity.GetType() })
                    .Invoke(service, new[] { entity });
            }
            catch
            {
                if (action == Action.Insert)
                {
                    Record(entity, Action.UnDelete, service, userId, utcOffset);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
