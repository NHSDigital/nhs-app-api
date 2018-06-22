using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord.Model;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord
{
    public class EmisProblemMapper
    {
        private const string DATE_FORMAT = "d MMMM yyyy";
        
        public Problems Map(MedicationRootObject problemsGetResponse)
        {
            var problems = new Problems();
            
            if (problemsGetResponse.MedicalRecord != null)
            {
                var medicalRecord = problemsGetResponse.MedicalRecord;
                var problemData = new List<ProblemItem>();

                if (medicalRecord.Problems != null)
                {
                    foreach (var problem in medicalRecord.Problems)
                    {
                        var problemItem = new ProblemItem();
                        problemItem.EffectiveDate = problem.Observation.EffectiveDate != null
                            ? new Date
                            {
                                Value = problem.Observation.EffectiveDate.Value,
                                DatePart = problem.Observation.EffectiveDate.DatePart
                            }
                            : null;
                        problemItem.LineItems = new List<ProblemLineItem>();
                        problemItem.LineItems.Add(new ProblemLineItem {Text = problem.Observation.Term});
                        problemItem.LineItems.Add(new ProblemLineItem {Text = "Significance: " + problem.Significance});
                        problemItem.LineItems.Add(new ProblemLineItem {Text = "Status: " + problem.Status});

                        if (problem.Observation.AssociatedText != null)
                        {
                            problemItem.LineItems.Add(new ProblemLineItem
                            {
                                Text = "Notes:", 
                                LineItems = problem.Observation.AssociatedText.Select(x => x.Text).ToList(),
                            });                            
                        }

                        if (problem.ProblemEndDate != null)
                        {
                            problemItem.LineItems.Add(new ProblemLineItem
                            {
                                Text = "Ended: " + problem.ProblemEndDate.Value.ToString(DATE_FORMAT)
                            });                           
                        }

                        problemData.Add(problemItem);
                    }

                    problems.Data = problemData;
                }
            }

            return problems;
        }
    }
}