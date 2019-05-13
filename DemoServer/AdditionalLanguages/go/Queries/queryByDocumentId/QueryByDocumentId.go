package main

//region Usings
import "github.com/ravendb/ravendb-go-client"
//endregion

var globalDocumentStore *ravendb.DocumentStore

func main() {
    createDocumentStore()
    createDatabase()
    queryByDocumentID("employees/8-A")
    globalDocumentStore.Close()
}

func createDocumentStore() (*ravendb.DocumentStore, error) {
    if globalDocumentStore != nil {
        return globalDocumentStore, nil
    }
    urls := []string{"http://localhost:8080"}
    store := ravendb.NewDocumentStore(urls, "testGO")
    err := store.Initialize()
    if err != nil {
        return nil, err
    }
    globalDocumentStore = store
    return globalDocumentStore, nil
}

func createDatabase() {
    databaseRecord := ravendb.NewDatabaseRecord()
    databaseRecord.DatabaseName = "testGO"
    createDatabaseOperation := ravendb.NewCreateDatabaseOperation(databaseRecord, 1)
    var err = globalDocumentStore.Maintenance().Server().Send(createDatabaseOperation)
    if err != nil {
        fmt.Printf("d.store.Maintenance().Server().Send(createDatabaseOperation) failed with %s\n", err)
    }
}

type Employee struct {
    ID         string
    LastName   string
    FirstName  string
    Title      string
}

//region Demo
func queryByDocumentID(employeeDocumentID string) error {

    session, err := globalDocumentStore.OpenSession("")
    if err != nil {
        return err
    }
    defer session.Close()

    //region Step_1
    queriedType := reflect.TypeOf(&Employee{})
    queryByDocumentID := session.QueryCollectionForType(queriedType)
    //endregion
    
    //region Step_2
    queryByDocumentID = queryByDocumentID.Where("ID", "==", employeeDocumentID)
    //endregion

    //region Step_3
    var employee *Employee
    err = queryByDocumentID.Single(&employee)
    if err != nil {
        return err
    }
    //endregion

    return nil
}
//endregion