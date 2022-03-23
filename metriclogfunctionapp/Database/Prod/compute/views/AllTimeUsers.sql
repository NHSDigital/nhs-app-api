CREATE OR REPLACE VIEW compute."AllTimeUsers" AS
    SELECT
        coalesce(sum("compute"."DailyUsage"."AcceptedTermsAndConditions"), 0) AS "AllTimeAppUsers",
        coalesce(sum("compute"."DailyUsage"."P9VerifiedNHSAppUsers"), 0) AS "AllTimeP9AppUsers"
   FROM "compute"."DailyUsage";

CALL perms.apply_etl_select_permissions('compute', 'AllTimeUsers');
