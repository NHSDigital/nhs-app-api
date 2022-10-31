-- Table: compute.GPRecordViews
DO $$
BEGIN

CREATE TABLE IF NOT EXISTS compute."GPRecordViews"
(
    "OdsCode" character varying COLLATE pg_catalog."default",
    "Date" date,
    "Supplier" character varying COLLATE pg_catalog."default",
    "HealthRecordViews" integer,
    "UniqueUsers" integer,
    "ViewsWithSummaryRecordAccess" integer,
    "ViewsWithDetailedRecordAccess" integer,
    "IsActingOnBehalfOfAnother" integer,
    "AllergiesAdverseReactionsSectionViewCount" integer DEFAULT 0,
    "ConsultationEventsSectionViewCount" integer DEFAULT 0,
    "DiagnosisSectionViewCount" integer DEFAULT 0,
    "DocumentsSectionViewCount" integer DEFAULT 0,
    "ExamFindingsSectionViewCount" integer DEFAULT 0,
    "HealthConditionsSectionViewCount" integer DEFAULT 0,
    "ImmunisationsSectionViewCount" integer DEFAULT 0,
    "MedicinesSectionViewCount" integer DEFAULT 0,
    "ProceduresSectionViewCount" integer DEFAULT 0,
    "TestResultsSectionViewCount" integer DEFAULT 0,
    CONSTRAINT gprecordviews_unique UNIQUE ("OdsCode", "Date", "Supplier")
);

CALL perms.apply_compute_table_permissions('compute', 'GPRecordViews');

END$$;