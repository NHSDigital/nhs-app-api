package cosmosSql

data class SqlRepositoryRecordUserAndInfo<SqlRepositoryUserAndInfo>(
        override val partitionKeyValue: String,
        override val id: String,
        override val repositoryRecord: SqlRepositoryUserAndInfo) : ISqlRepositoryRecord<SqlRepositoryUserAndInfo>

