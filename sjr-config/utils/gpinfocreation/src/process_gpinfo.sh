#!/bin/bash

function usage () {
   echo -e "\nUsage:\nprocess_gpinfo.sh [--new-practices] \n"
}

PWD_OPTION=""
if [[ "$OSTYPE" == "msys"* ]]; then
    PWD_OPTION="-W"
fi
BASE_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )/../" && pwd $PWD_OPTION )"
CONFIGURATION_DIR="$BASE_DIR/../../configurations"
FULL_GP_INFO="$BASE_DIR/data/full_gpinfo.csv"
GP_INFO="$CONFIGURATION_DIR/gpinfo.csv"
NEW_GP_INFO="$CONFIGURATION_DIR/new_gpinfo.csv"
EXCLUDED_PRACTICES="$BASE_DIR/data/excluded-practices.csv"

CHANGED_SUPPLIERS="$BASE_DIR/data/changedsuppliers.txt"
NEW_CHANGED_SUPPLIERS="$BASE_DIR/data/new_changedsuppliers.txt"
MISSING_PRACTICES="$BASE_DIR/data/missingpractices.txt"
NEW_MISSING_PRACTICES="$BASE_DIR/data/new_missingpractices.txt"
PENDING_UPDATES="$BASE_DIR/data/pendingupdates.txt"
NEW_PENDING_UPDATES="$BASE_DIR/data/new_pendingupdates.txt"
JOB_RUN=`date`
EXCLUDE_NEW_PRACTICES=0

while :; do
    case $1 in
        -n|--no-new) EXCLUDE_NEW_PRACTICES=1
        ;;
        -h|--help) usage; exit 1
        ;;
        *) break
    esac
    shift
done


sqlite3 <<EOF

.mode csv


.print Reading the full GP Info from $FULL_GP_INFO
.import "$FULL_GP_INFO" full_gpinfo

.print Reading the enabled GP Info from $GP_INFO
.import "$GP_INFO" current_gpinfo

.print Reading the excluded GP Practices from $EXCLUDED_PRACTICES
.import "$EXCLUDED_PRACTICES" excluded_practices

.print De-duplicating full GP Info
create table deduped_full_gpinfo_with_latest_supplier as
    select      a.*
    from        full_gpinfo a
    inner join  (
        select  ODS,
                max(EndpointCreated) maxdate
        from    full_gpinfo
        where   ClosedDate = ""
                and ODS not in (select ODS from excluded_practices)
        group by ODS
    ) b on  a.ODS = b.ODS
        and a.EndpointCreated = b.maxdate;

.print Removing practices that have not been fully  updated
create table deduped_practices_with_no_extension as
    select ODS, Version, Supplier, EndpointCreated
    from deduped_full_gpinfo_with_latest_supplier
    where Version not LIKE '%EPS2%'
          and Version not like '%GPES%'
          and Version not like '%GP2GP%' ;

create table practices_not_updated as
    select a.*
    from full_gpinfo a
    inner join (
        select ODS, Version, Supplier, EndpointCreated
        from deduped_practices_with_no_extension
    ) b on  a.ODS = b.ODS
            and a.Supplier != b.Supplier
            and (a.Version like '%EPS2%' or a.Version like '%GPES%' or a.Version like '%GP2GP%');



create table pending_update_details as
    select a.ODS as ODS, b.Supplier as OldSupplier, a.Supplier as NewSupplier
    from deduped_full_gpinfo_with_latest_supplier a
    inner join (
        select *
        from practices_not_updated
    ) b on  a.ODS = b.ODS;

.output "$NEW_PENDING_UPDATES"
.print ==================================================================
.print Job Run - $JOB_RUN
.headers on
select * FROM pending_update_details
order by ODS;
.print ==================================================================
.headers off
.output

.print
.print Practices pending an update:
select count(*) from pending_update_details;


create table deduped_full_gpinfo as
    select *
    from deduped_full_gpinfo_with_latest_supplier
    where ODS not in (select ODS from practices_not_updated)
    union
    select * from practices_not_updated;


.print
.print Compiling the new GP Info list to be enabled
create table new_gpinfo as
    select  *
    from    deduped_full_gpinfo
    where   Supplier in (
                "EGTON MEDICAL INFORMATION SYSTEMS LTD (EMIS)",
                "THE PHOENIX PARTNERSHIP",
                "MICROTEST LTD",
                "IN PRACTICE SYSTEMS LTD"
            )
    and     ClosedDate="" order by Supplier, CCG, Organisation;


create table supplier_changes as
select      new.ODS,
            new.Organisation,
            old.Supplier as PreviousSupplier,
            new.Supplier as NewSupplier
from        new_gpinfo new
inner join  current_gpinfo old
        on  new.ODS = old.ODS
       and  new.Supplier != old.Supplier;

.output "$NEW_CHANGED_SUPPLIERS"
.print ==================================================================
.print Job Run - $JOB_RUN
.headers on
select      *
from        supplier_changes
order by    ODS;
.print ==================================================================
.headers off
.output

.print 
.print Supplier Changes:
select count(*) from supplier_changes;



create table new_practices as
select      ODS,
            Organisation,
            Supplier
from        new_gpinfo
where       ODS not in (select ODS from current_gpinfo);

.print 
.print New Practices:
select count(*) from new_practices;



$(if [ "$EXCLUDE_NEW_PRACTICES" == 1 ]; then echo ".print
.print Removing new practices from the new GP Info list
.print
.print Practices before removing new practices:
select count(*) from new_gpinfo;

delete
from        new_gpinfo
where       ODS not in (select ODS from current_gpinfo);

.print Practices after removing new practice:
select count(*) from new_gpinfo;"; fi)



create table missing_practices as
select  *
from    current_gpinfo
where   ODS not in (select ODS from new_gpinfo)
        and ODS not in (select ODS from excluded_practices);

.output "$NEW_MISSING_PRACTICES"
.print ==================================================================
.print Job Run - $JOB_RUN
.headers on
select      *
from        missing_practices
order by    ODS;
.print ==================================================================
.headers off
.output

.print 
.print Missing Practices:
select count(*) from missing_practices;


.print
.print Adding missing practices back to new GP Info list
insert into new_gpinfo
select  * from missing_practices;



.print Writing new GP Info list to $NEW_GP_INFO
.output "$NEW_GP_INFO"
.headers on
select      *
from        new_gpinfo
order by    ODS;
.output

.print 


EOF

mv "$NEW_GP_INFO" "$GP_INFO"

cat "$NEW_CHANGED_SUPPLIERS" >> "$CHANGED_SUPPLIERS"
rm "$NEW_CHANGED_SUPPLIERS"

cat "$NEW_MISSING_PRACTICES" >> "$MISSING_PRACTICES"
rm "$NEW_MISSING_PRACTICES"

cat "$NEW_PENDING_UPDATES" >> "$PENDING_UPDATES"
rm "$NEW_PENDING_UPDATES"

