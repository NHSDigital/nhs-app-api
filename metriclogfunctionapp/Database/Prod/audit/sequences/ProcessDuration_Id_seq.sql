CREATE SEQUENCE IF NOT EXISTS audit."ProcessDuration_Id_seq"
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 2147483647
    CACHE 1;
CALL perms.apply_sequence_permissions('audit', 'ProcessDuration', 'Id');
