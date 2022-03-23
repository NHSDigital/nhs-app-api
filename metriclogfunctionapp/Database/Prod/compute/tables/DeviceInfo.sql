CREATE TABLE IF NOT EXISTS compute."DeviceInfo"
(
    "Date" date,
    "AppVersion" character varying COLLATE pg_catalog."default",
    "DeviceManufacturer" character varying COLLATE pg_catalog."default",
    "DeviceModel" character varying COLLATE pg_catalog."default",
    "DeviceOS" character varying COLLATE pg_catalog."default",
    "DeviceOSVersion" character varying COLLATE pg_catalog."default",
    "Access" character varying COLLATE pg_catalog."default",
    "Sessions" bigint,
    CONSTRAINT dupes_pkey UNIQUE ("Date", "AppVersion", "DeviceManufacturer", "DeviceModel", "DeviceOS", "DeviceOSVersion", "Access")
);

CALL perms.apply_compute_table_permissions('compute', 'DeviceInfo');