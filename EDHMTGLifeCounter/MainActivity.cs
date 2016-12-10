using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Views;
using Android.Views.InputMethods;
using Android.Content;
using System.Linq;
using System.IO;

namespace EDHMTGLifeCounter
{
    [Activity(Label = "EDHMTGLifeCounter", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private ViewModel.LifeCounter mainActivityModel = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            ActionBar.NavigationMode = ActionBarNavigationMode.Tabs;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.LifeCounter);

            SqliteConnection conn = new SqliteConnection();
            conn.SQLiteConnection();
            conn.CreateTables();

            var model = conn.GetLatestEntry();
            if (model == null || model.Life == 0)
            {
                ViewModel.LifeCounter lifeModel = new ViewModel.LifeCounter();
                lifeModel.CommanderDamageCounter = 0;
                lifeModel.PoisonDamageCounter = 0;
                lifeModel.Life = 40;
                var returnModel = conn.AddLifeCounter(Translate(lifeModel));
                mainActivityModel = Translate(returnModel);
            }
            else
            {
                mainActivityModel = Translate(model);
            }

            SetupTabs(conn, model);
            
            ActionBar.Title = "EDH MTG Life Counter";
        }

        private void SetupTabs(SqliteConnection conn, Model.ModelLifeCounter model)
        {
            ActionBar.Tab tab = ActionBar.NewTab();
            tab.SetText("Life");

            tab.TabSelected += (sender, args) =>
            {
                conn.UpdateLifeCounter(Translate(mainActivityModel));
                SetContentView(Resource.Layout.LifeCounter);
                model = conn.GetLatestEntry();
                SetupLifeMainView(Translate(model));
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();
            tab.SetText("Poison");

            tab.TabSelected += (sender, args) =>
            {
                conn.UpdateLifeCounter(Translate(mainActivityModel));
                SetContentView(Resource.Layout.layPoison);
                model = conn.GetLatestEntry();
                SetupLifeMainView(Translate(model));
            };
            ActionBar.AddTab(tab);

            tab = ActionBar.NewTab();

            tab.SetText("Commander");

            tab.TabSelected += (sender, args) =>
            {
                conn.UpdateLifeCounter(Translate(mainActivityModel));
                SetContentView(Resource.Layout.layCommander);
                model = conn.GetLatestEntry();
                SetupLifeMainView(Translate(model));
            };
            ActionBar.AddTab(tab);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_reset:
                    SqliteConnection conn = new SqliteConnection();
                    conn.SQLiteConnection();
                    ResetAll(conn);
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        private void ResetAll(SqliteConnection conn)
        {
            Dialog dia = new Dialog(this);
            dia = new Dialog(this);
            dia.RequestWindowFeature((int)WindowFeatures.NoTitle);
            dia.SetContentView(Resource.Layout.layAreYouSure);

            Button cancelButton = dia.FindViewById<Button>(Resource.Id.btnCancelDiag);
            cancelButton.Click += delegate { dia.Dismiss(); };

            Button okayButton = dia.FindViewById<Button>(Resource.Id.btnOkayDiag);
            okayButton.Click += delegate
            {
                TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                
                mainActivityModel.Life = 40;
                mainActivityModel.PoisonDamageCounter = 0;
                mainActivityModel.CommanderDamageCounter = 0;
                if (ActionBar.SelectedTab.Position == 0)
                {
                    txtHelth.Text = mainActivityModel.Life.ToString();
                }
                else if (ActionBar.SelectedTab.Position == 1)
                {
                    txtHelth.Text = mainActivityModel.PoisonDamageCounter.ToString();
                }
                else if (ActionBar.SelectedTab.Position == 2)
                {
                    txtHelth.Text = mainActivityModel.CommanderDamageCounter.ToString();
                }
                conn.UpdateLifeCounter(Translate(mainActivityModel));

                dia.Dismiss();
            };
            dia.Show();
        }

        private void SetupLifeMainView(ViewModel.LifeCounter ViewModel)
        {
            SqliteConnection conn = new SqliteConnection();
            conn.SQLiteConnection();

            TextView txthealt = FindViewById<TextView>(Resource.Id.txtHealth);
            Button addOneButton = FindViewById<Button>(Resource.Id.btnAddOne);
            Button subtractOneButton = FindViewById<Button>(Resource.Id.btnSubtractOne);
            
            TextView counterText = FindViewById<TextView>(Resource.Id.txtHealth);
            if (ActionBar.SelectedTab.Position == 0)
            {
                counterText.Text = ViewModel.Life.ToString();
            }
            else if (ActionBar.SelectedTab.Position == 1)
            {
                counterText.Text = ViewModel.PoisonDamageCounter.ToString();

            }
            else if (ActionBar.SelectedTab.Position == 2)
            {
                counterText.Text = ViewModel.CommanderDamageCounter.ToString();

            }
            txthealt.Click += delegate { AddFiveHealth(conn); };
            addOneButton.Click += delegate { AddOneHealth(mainActivityModel, conn); };
            subtractOneButton.Click += delegate { SubtractOneHealth(mainActivityModel, conn); };

        }

        private int ConvertPixelsToDp(float pixelValue)
        {
            var dp = (int)((pixelValue) / Resources.DisplayMetrics.Density);
            return dp;
        }

        private void AddFiveHealth(SqliteConnection conn)
        {
            var selectedTab = ActionBar.SelectedTab;

            Dialog dia = new Dialog(this);
            dia = new Dialog(this);
            dia.SetTitle("Enter amount (Ex: -5 or 5): ");
            dia.SetContentView(Resource.Layout.layAmount);
            Button cancelButton = dia.FindViewById<Button>(Resource.Id.btnCancelDialog);
            Button addAmountButton = dia.FindViewById<Button>(Resource.Id.btnAddAmountDialog);
            EditText amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            cancelButton.Click += delegate
            {
                HideKeyboard(amount, imm);
                dia.Dismiss();
            };
            if (selectedTab.Position == 0)
            {
                addAmountButton.Click += delegate
                {

                    amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                    TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                    mainActivityModel.Life = Convert.ToInt16(amount.Text) + Convert.ToInt32(txtHelth.Text);
                    if (mainActivityModel.Life <= 0)
                    {
                        txtHelth.Text = (0).ToString();
                        mainActivityModel.Life = 0;
                    }
                    else
                    {
                        txtHelth.Text = (mainActivityModel.Life).ToString();
                    }
                    conn.UpdateLifeCounter(Translate(mainActivityModel));

                    HideKeyboard(amount, imm);
                    dia.Dismiss();
                };
                dia.Show();
                ShowKeyboard(amount, imm);
                dia.DismissEvent += delegate
                {
                    HideKeyboard(amount, imm);
                };
            }
            else if (selectedTab.Position == 1)
            {
                addAmountButton.Click += delegate
                {
                    amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                    TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                    mainActivityModel.PoisonDamageCounter = Convert.ToInt16(amount.Text) + Convert.ToInt32(txtHelth.Text);
                    if (mainActivityModel.PoisonDamageCounter <= 0)
                    {
                        txtHelth.Text = (0).ToString();
                        mainActivityModel.PoisonDamageCounter = 0;
                    }
                    else
                    {
                        txtHelth.Text = (mainActivityModel.PoisonDamageCounter).ToString();
                    }
                    conn.UpdateLifeCounter(Translate(mainActivityModel));

                    HideKeyboard(amount, imm);
                    dia.Dismiss();
                };
                dia.Show();
                ShowKeyboard(amount, imm);
                dia.DismissEvent += delegate
                {
                    HideKeyboard(amount, imm);
                };
            }
            else
            {
                addAmountButton.Click += delegate
                {
                    amount = dia.FindViewById<EditText>(Resource.Id.txtAmountDialog);
                    TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
                    mainActivityModel.CommanderDamageCounter = Convert.ToInt16(amount.Text) + Convert.ToInt32(txtHelth.Text);
                    if (mainActivityModel.CommanderDamageCounter <= 0)
                    {
                        txtHelth.Text = (0).ToString();
                        mainActivityModel.CommanderDamageCounter = 0;
                    }
                    else
                    {
                        txtHelth.Text = (mainActivityModel.CommanderDamageCounter).ToString();
                    }
                    conn.UpdateLifeCounter(Translate(mainActivityModel));

                    HideKeyboard(amount, imm);
                    dia.Dismiss();
                };
                dia.Show();
                ShowKeyboard(amount, imm);
                dia.DismissEvent += delegate
                {
                    HideKeyboard(amount, imm);
                };
            }
        }

        public static void ShowKeyboard(View pView, InputMethodManager imm)
        {
            pView.RequestFocus();

            imm.ShowSoftInput(pView, ShowFlags.Forced);
            imm.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        }

        public static void HideKeyboard(View pView, InputMethodManager imm)
        {

            pView.ClearFocus();

            imm.HideSoftInputFromWindow(pView.WindowToken, HideSoftInputFlags.None);
        }

        private void AddOneHealth(ViewModel.LifeCounter ViewModel, SqliteConnection conn)
        {
            TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
            if (ActionBar.SelectedTab.Position == 0)
            {
                mainActivityModel.Life += 1;
                txtHelth.Text = mainActivityModel.Life.ToString();
                conn.UpdateLifeCounter(Translate(mainActivityModel));

            }
            else if (ActionBar.SelectedTab.Position == 1)
            {
                mainActivityModel.PoisonDamageCounter += 1;
                txtHelth.Text = mainActivityModel.PoisonDamageCounter.ToString();
                conn.UpdateLifeCounter(Translate(mainActivityModel));

            }
            else if (ActionBar.SelectedTab.Position == 2)
            {
                mainActivityModel.CommanderDamageCounter += 1;
                txtHelth.Text = mainActivityModel.CommanderDamageCounter.ToString();
                conn.UpdateLifeCounter(Translate(mainActivityModel));

            }
        }

        private void SubtractOneHealth(ViewModel.LifeCounter ViewModel, SqliteConnection conn)
        {
            TextView txtHelth = FindViewById<TextView>(Resource.Id.txtHealth);
            if (ActionBar.SelectedTab.Position == 0)
            {
                mainActivityModel.Life -= 1;
                if (mainActivityModel.Life <= 0)
                {
                    mainActivityModel.Life = 0;
                    txtHelth.Text = (0).ToString();
                }
                else
                {
                    txtHelth.Text = (mainActivityModel.Life).ToString();
                }
                conn.UpdateLifeCounter(Translate(mainActivityModel));


            }
            else if (ActionBar.SelectedTab.Position == 1)
            {
                mainActivityModel.PoisonDamageCounter -= 1;
                if (mainActivityModel.PoisonDamageCounter <= 0)
                {
                    mainActivityModel.PoisonDamageCounter = 0;
                    txtHelth.Text = (0).ToString();
                }
                else
                {
                    txtHelth.Text = (mainActivityModel.PoisonDamageCounter).ToString();
                }
                conn.UpdateLifeCounter(Translate(mainActivityModel));

            }
            else if (ActionBar.SelectedTab.Position == 2)
            {
                mainActivityModel.CommanderDamageCounter -= 1;
                if (mainActivityModel.CommanderDamageCounter <= 0)
                {
                    mainActivityModel.CommanderDamageCounter = 0;
                    txtHelth.Text = (0).ToString();
                }
                else
                {
                    txtHelth.Text = (mainActivityModel.CommanderDamageCounter).ToString();
                }
                conn.UpdateLifeCounter(Translate(mainActivityModel));

            }

        }
        
        private void CheckIfLost(int selectedTab)
        {


        }

        private ViewModel.LifeCounter Translate(Model.ModelLifeCounter model)
        {
            ViewModel.LifeCounter returnView = new ViewModel.LifeCounter();
            returnView.CommanderDamageCounter = model.CommanderDamageCounter;
            returnView.CreateDate = model.CreateDate;
            returnView.Deleted = model.Deleted;
            returnView.Id = model.Id;
            returnView.Inserted = model.Inserted;
            returnView.Life = model.Life;
            returnView.PoisonDamageCounter = model.PoisonDamageCounter;
            returnView.Updated = model.Updated;
            returnView.UpdateDate = model.UpdateDate;
            returnView.User = model.User;

            return returnView;
        }

        private Model.ModelLifeCounter Translate(ViewModel.LifeCounter dataModel)
        {
            Model.ModelLifeCounter model = new Model.ModelLifeCounter();
            model.CommanderDamageCounter = dataModel.CommanderDamageCounter;
            model.CreateDate = dataModel.CreateDate;
            model.Deleted = dataModel.Deleted;
            model.Id = dataModel.Id;
            model.Inserted = dataModel.Inserted;
            model.Life = dataModel.Life;
            model.PoisonDamageCounter = dataModel.PoisonDamageCounter;
            model.Updated = dataModel.Updated;
            model.UpdateDate = dataModel.UpdateDate;
            model.User = dataModel.User;

            return model;
        }
    }

    public class SqliteConnection
    {
        private SQLite.SQLiteConnection conn = null;
        public void SQLiteConnection()
        {
            var pathForDB = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "db1.sqlite");
            conn = new SQLite.SQLiteConnection(pathForDB, false);
        }

        public void CreateTables()
        {
            conn.CreateTable<Model.ModelLifeCounter>();
        }

        public Model.ModelLifeCounter AddLifeCounter(Model.ModelLifeCounter data)
        {
            Model.ModelLifeCounter addModel = new Model.ModelLifeCounter();
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

        public Model.ModelLifeCounter UpdateLifeCounter(Model.ModelLifeCounter data)
        {
            conn.CreateCommand("Select * From ModelLifeCounter where Id = " + data.Id.ToString());

            var model = conn.Query<Model.ModelLifeCounter>("Select * From ModelLifeCounter where Id = " + data.Id.ToString()).SingleOrDefault();
            model.Deleted = false;
            model.CommanderDamageCounter = data.CommanderDamageCounter;
            model.Life = data.Life;
            model.PoisonDamageCounter = data.PoisonDamageCounter;
            model.Updated = true;
            model.UpdateDate = DateTime.Now;
            model.User = "TestUpdated";
            conn.Update(model);
            conn.Commit();

            Model.ModelLifeCounter updatedModel = new Model.ModelLifeCounter();
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

        public Model.ModelLifeCounter GetLatestEntry()
        {
            var model = conn.Query<Model.ModelLifeCounter>("Select * From ModelLifeCounter").OrderByDescending(x => x.CreateDate).FirstOrDefault();

            Model.ModelLifeCounter updatedModel = new Model.ModelLifeCounter();
            if (model != null)
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

