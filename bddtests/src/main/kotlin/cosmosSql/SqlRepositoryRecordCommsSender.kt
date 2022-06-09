package cosmosSql

data class SqlRepositoryRecordCommsSender<SqlRepositoryCommsSender>(
    override val partitionKeyValue: String,
    override val id: String,
    override val repositoryRecord: SqlRepositoryCommsSender) : ISqlRepositoryRecord<SqlRepositoryCommsSender>

