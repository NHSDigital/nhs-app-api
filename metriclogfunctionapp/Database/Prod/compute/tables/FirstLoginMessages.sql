CREATE TABLE IF NOT EXISTS compute."FirstLoginMessages" (
	"MessageId" character varying(36) NOT NULL UNIQUE,
	"LoginId" character varying(36) NOT NULL,
	"MessageSendTimestamp" timestamp with time zone NOT NULL,
	"UserFirstLoginTimestamp" timestamp with time zone NOT NULL
);

CALL perms.apply_compute_table_permissions('compute', 'FirstLoginMessages');