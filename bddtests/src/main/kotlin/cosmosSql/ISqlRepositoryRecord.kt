package cosmosSql

interface ISqlRepositoryRecord<T>{
    val partitionKeyValue: String
    val id: String
    val repositoryRecord: T
}
