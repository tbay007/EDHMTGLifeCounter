using System;
using System.Linq;
using System.IO;
using System.Data;
namespace EDHMTGDataAccess
{
    public class SqliteConnection
    {
        private SQLite.SQLiteConnection conn = null;
        public void SQLiteConnection()
        {           
            var pathForDB = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "db.sqlite"); 
            conn = new SQLite.SQLiteConnection(pathForDB, false);
        }

        public void CreateTables()
        {
            conn.CreateTable<Model.LifeCounter>();
        }

        public Model.LifeCounter AddLifeCounter(Model.LifeCounter data)
        {
            Model.LifeCounter addModel = new Model.LifeCounter();
            addModel.CommanderDamageCounter = data.CommanderDamageCounter;
            addModel.CreateDate = DateTime.Now;
            addModel.Inserted = true;
            addModel.Life = data.Life;
            addModel.PoisonDamageCounter = data.PoisonDamageCounter;
            addModel.Updated = false;
            addModel.UpdateDate = DateTime.Now;
            addModel.User = "Test";
            conn.Insert(addModel);
            conn.Commit();

            data.Id = addModel.Id;
            return data;
        }

        public Model.LifeCounter UpdateLifeCounter(Model.LifeCounter data)
        {
            conn.CreateCommand("Select * From LifeCounter where Id = " + data.Id.ToString());
            
            var model = conn.Query<Model.LifeCounter>("Select * From LifeCounter where Id = " + data.Id.ToString()).SingleOrDefault();
            model.Deleted = false;
            model.CommanderDamageCounter = data.CommanderDamageCounter;
            model.Life = data.Life;
            model.PoisonDamageCounter = data.PoisonDamageCounter;
            model.Updated = true;
            model.UpdateDate = DateTime.Now;
            model.User = "TestUpdated";
            conn.Update(model);
            conn.Commit();

            Model.LifeCounter updatedModel = new Model.LifeCounter();
            updatedModel.Id = model.Id;
            updatedModel.Inserted = model.Inserted;
            updatedModel.CommanderDamageCounter = model.CommanderDamageCounter;
            updatedModel.CreateDate = model.CreateDate;
            updatedModel.Deleted = model.Deleted;
            updatedModel.Life = model.Life;
            updatedModel.PoisonDamageCounter = model.PoisonDamageCounter;
            updatedModel.Updated = model.Updated;
            updatedModel.UpdateDate = model.UpdateDate;
            updatedModel.User = model.User;
            return updatedModel;
        }

        public Model.LifeCounter GetLatestEntry()
        {
            var model = conn.Query<Model.LifeCounter>("Select * From LifeCounter").OrderByDescending(x => x.CreateDate).FirstOrDefault();

            Model.LifeCounter updatedModel = new Model.LifeCounter();
            if (model !=null)
            {
                updatedModel.Id = model.Id;
                updatedModel.Inserted = model.Inserted;
                updatedModel.CommanderDamageCounter = model.CommanderDamageCounter;
                updatedModel.CreateDate = model.CreateDate;
                updatedModel.Deleted = model.Deleted;
                updatedModel.Life = model.Life;
                updatedModel.PoisonDamageCounter = model.PoisonDamageCounter;
                updatedModel.Updated = model.Updated;
                updatedModel.UpdateDate = model.UpdateDate;
                updatedModel.User = model.User;
            }
           
            return updatedModel;
        }
    }
}
