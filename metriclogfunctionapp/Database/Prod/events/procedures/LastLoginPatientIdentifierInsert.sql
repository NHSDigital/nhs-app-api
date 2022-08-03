CREATE OR REPLACE PROCEDURE events.LastLoginPatientIdentifierInsert (
    loginId character varying(36),
    nhsNumber character varying (36),
    lastLoginTimestamp timestamp with time zone,
    auditId character varying(36)
)
AS $$

BEGIN

    IF NOT EXISTS (SELECT FROM events."LastLoginPatientIdentifier" WHERE "AuditId" = auditId) THEN

        INSERT INTO "events"."LastLoginPatientIdentifier" ("LoginId", "NhsNumber", "Timestamp", "AuditId")
        VALUES (loginId, nhsNumber, lastLoginTimestamp, auditId)
        ON CONFLICT ON CONSTRAINT lastloginpatientidentifier_loginidnhsnumber_pk
        DO UPDATE SET "Timestamp" = Excluded."Timestamp";

    END IF;

END;

$$ LANGUAGE plpgsql;