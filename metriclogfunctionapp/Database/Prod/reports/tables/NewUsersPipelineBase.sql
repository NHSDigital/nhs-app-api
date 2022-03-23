
CREATE TABLE IF NOT EXISTS reports."NewUsersPipelineBase" (
    "Date" timestamp(6) without time zone,
    "PracticeCode" character varying,
    "P5NewAppUsers" integer,
    "P9SuccessfulUplifts" integer,
    "AcceptedTermsAndConditions" integer
);
