using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

public class GPRecordViewsRows : ITableRow
{
    public string OdsCode { get; set; }
    public DateTime Date { get; set; }
    public string Supplier { get; set; }
    public int HealthRecordViews { get; set; }
    public int UniqueUsers { get; set; }
    public int ViewsWithSummaryRecordAccess { get; set; }
    public int ViewsWithDetailedRecordAccess { get; set; }
    public int IsActingOnBehalfOfAnother { get; set; }
    public int AllergiesAdverseReactionsSectionViewCount { get; set; }
    public int ConsultationEventsSectionViewCount { get; set; }
    public int DiagnosisSectionViewCount { get; set; }
    public int DocumentsSectionViewCount { get; set; }
    public int ExamFindingsSectionViewCount { get; set; }
    public int HealthConditionsSectionViewCount { get; set; }
    public int ImmunisationsSectionViewCount { get; set; }
    public int MedicinesSectionViewCount { get; set; }
    public int ProceduresSectionViewCount { get; set; }
    public int TestResultsSectionViewCount { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""OdsCode"", ""Date"", ""Supplier"", ""HealthRecordViews"", ""UniqueUsers"", ""ViewsWithSummaryRecordAccess"",
                            ""ViewsWithDetailedRecordAccess"", ""IsActingOnBehalfOfAnother"", ""AllergiesAdverseReactionsSectionViewCount"",
                            ""ConsultationEventsSectionViewCount"", ""DiagnosisSectionViewCount"", ""DocumentsSectionViewCount"",
                            ""ExamFindingsSectionViewCount"", ""HealthConditionsSectionViewCount"", ""ImmunisationsSectionViewCount"",
                            ""MedicinesSectionViewCount"", ""ProceduresSectionViewCount"",""TestResultsSectionViewCount"")
VALUES(@OdsCode, @Date, @Supplier, @HealthRecordViews, @UniqueUsers, @ViewsWithSummaryRecordAccess, @ViewsWithDetailedRecordAccess, @IsActingOnBehalfOfAnother, @AllergiesAdverseReactionsSectionViewCount,
       @ConsultationEventsSectionViewCount, @DiagnosisSectionViewCount, @DocumentsSectionViewCount, @ExamFindingsSectionViewCount, @HealthConditionsSectionViewCount,
       @ImmunisationsSectionViewCount, @MedicinesSectionViewCount, @ProceduresSectionViewCount, @TestResultsSectionViewCount)
";
}