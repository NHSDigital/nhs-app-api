CREATE TABLE IF NOT EXISTS audit."NhsLoginFigures" (
    "NhsLoginId" character varying(36) NOT NULL,
    "NhsNumber" character varying(36),
    "DeletedFlag" BOOLEAN,
    "CurrentOdsCode" character varying,
    "MonthYearOfBirth" DATE
);

GRANT SELECT,INSERT,TRUNCATE ON audit."NhsLoginFigures" TO "AzureNhsAnalytics_DataFactory";

CREATE INDEX IF NOT EXISTS NhsLoginFigures_NhsLoginId_idx on audit."NhsLoginFigures" ("NhsLoginId");
CREATE INDEX IF NOT EXISTS NhsLoginFigures_NhsNumber_idx on audit."NhsLoginFigures" ("NhsNumber");