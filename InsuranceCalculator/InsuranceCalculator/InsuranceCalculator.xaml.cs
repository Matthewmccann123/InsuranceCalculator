using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace InsuranceCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            NewPolicy.PolicyStartDate = DateTime.MinValue;
            AddDriverLabels();
        }
        int driverCount = 0;
        public int CurrentClaim = 0;
        public double policyMultiplier = 1.0;
        double initialPremium = 500.0;
        public int EditingDriverId = 0;
        public string EditingDriversName = "";
        public string EditingClaim = "";
        Driver CurrentDriver = new Driver();
        Policy NewPolicy = new Policy();
        public List<TextBlock> DriverInfo = new List<TextBlock>();
        List<DateTime> CurrentDriverClaims = new List<DateTime>();
        List<Driver> TotalDrivers = new List<Driver>();
        public Boolean EditMode = false;
        
        //Add initial list for DriverDetail labels.
        private void AddDriverLabels(){
             DriverInfo.Add(Driver1Details);
             DriverInfo.Add(Driver2Details);
             DriverInfo.Add(Driver3Details);
             DriverInfo.Add(Driver4Details);
             DriverInfo.Add(Driver5Details);
        }

        //Selecting and changing Policy start date from Calendar.
        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //Reference
            var calendar = sender as Calendar;

            //Check Value != Null (Shouldn't if it has changed)
            if (calendar.SelectedDate.HasValue)
            {
                // ... Display SelectedDate in Title.
                NewPolicy.PolicyStartDate = calendar.SelectedDate.Value;
                MessageBox.Show("You have selected your policy's start date to be: " + NewPolicy.PolicyStartDate.Date);
                if (NewPolicy.PolicyStartDate.Date < DateTime.Now.Date)
                {
                    NewPolicy.PolicyStartDate = DateTime.Now;
                    MessageBox.Show("You cannot select a policy start date before today. Defaulting to today.");
                }
            }
        }
        //Confirming the details of an entire driver, and adding him to the policy
        private void ConfirmDriver(object sender, MouseButtonEventArgs e)
        {
            if (NewPolicy.Drivers == null || NewPolicy.Drivers.Count < 5)
            {
                if (Occupations.SelectedValue == null)
                {
                    MessageBox.Show("Please select an occupation.");
                    return;
                }
                bool dateIsValid = ValidateDate(DriverDOB.Text);
                if (!dateIsValid)
                {
                    MessageBox.Show("Date of birth does not match 00/00/0000 format");
                    return;
                }
                CurrentDriver = new Driver(DriversName.Text, Occupations.SelectedValue.ToString(), Convert.ToDateTime(DriverDOB.Text), CurrentDriverClaims);
                if (EditMode)
                {
                    for (int i = 0; i < NewPolicy.Drivers.Count; i++)
                    {
                        if (NewPolicy.Drivers[i].Name == EditingDriversName)
                        {
                            for (int j = 0; j < NewPolicy.Drivers[i].Claims.Count; j++)
                            {
                                bool validDate = ValidateDate(NewPolicy.Drivers[i].Claims[j].ToString("dd/MM/yyyy"));
                                bool validDOB = ValidateDate(DriverDOB.Text);
                                bool validClaimAge = ValidateClaimsAge(NewPolicy.Drivers[i].Claims[j].ToString("dd/MM/yyyy"), DriverDOB.Text);
                                if (!validClaimAge)
                                {
                                    MessageBox.Show("Driver cannot claim before he's of driving age.");
                                    return;
                                }
                            }
                            NewPolicy.Drivers[i] = new Driver(DriversName.Text, Occupations.SelectedValue.ToString(), Convert.ToDateTime(DriverDOB.Text), NewPolicy.Drivers[i].Claims);
                            EditMode = false;                     
                            Update_Claim.IsEnabled = false;
                            UpdateDriverInfos();
                            ResetDriverUI();
                            MessageBox.Show("Driver " + NewPolicy.Drivers[i].Name + " details updated!");
                            return;
                        }
                    }
                }
                NewPolicy.Drivers.Add(CurrentDriver);
                driverCount++;
                List<Label> DriversLabels = new List<Label>();
                PolicyDrivers.Text = "This policy currently has " + NewPolicy.Drivers.Count + " drivers.";
                UpdateDriverInfos();
                TotalDrivers.Add(CurrentDriver);
                CalcPremBtn.IsEnabled = true;
                MessageBox.Show("Driver " + CurrentDriver.Name + " has been added to the Policy as a(n) " + CurrentDriver.Occupation);
                ResetDriverUI();
            }
            else
            {
                MessageBox.Show("The Maximum of number of drivers to a policy is 5.");
            }

        }

        //Updating the UI with new driver's information, or resetting driver info if policy has been discarded
        private void UpdateDriverInfos()
        {
            if (NewPolicy.Drivers.Count == 0)
            {
                DriverInfo[0].Text = "";
                DriverInfo[1].Text = "";
                DriverInfo[2].Text = "";
                DriverInfo[3].Text = "";
                DriverInfo[4].Text = "";
            }
            for (int i = 0; i < NewPolicy.Drivers.Count; i++)
            {
                DriverInfo[i].Text = "Driver " + i + " details: \n   Name: " + NewPolicy.Drivers[i].Name + " \n   Occupation: "
                    + NewPolicy.Drivers[i].Occupation + " \n   Date of Birth: " + NewPolicy.Drivers[i].DateOfBirth.Date + " \n   Number of claims: " + NewPolicy.Drivers[i].Claims.Count;
            }
        }

        //Ensure date strings are in correct datetime format
        private bool ValidateDate(string dateEntered)
        {
            DateTime tempDate;
            bool validDT = DateTime.TryParse(dateEntered, out tempDate) ? true : false;
            return validDT;
        }

        //Ensure Occupation is updated when changed in UI
        private void OccupationChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentDriver.Occupation != null)
            {
           //     CurrentDriver.Occupation = Occupations.SelectedValue.ToString();
            }
        }

        //Showing Options for adding Claims in the UI
        private void Enable_Claims(object sender, MouseButtonEventArgs e)
        {
            Claim.Visibility = System.Windows.Visibility.Visible;
            ConfirmClaim.Visibility = System.Windows.Visibility.Visible;
        }

        //Adding a Claim to a specific Driver
        private void AddDriverClaim(object sender, MouseButtonEventArgs e)
        {
           
            bool dateIsValid = ValidateDate(Claim.Text);
            bool dobIsValid = ValidateDate(DriverDOB.Text);
            if (dateIsValid == false || dobIsValid == false)
            {
                MessageBox.Show("Claim or Driver's Date of birth does not match 00/00/0000 format");
                return;
            }

            bool validClaimAge = ValidateClaimsAge(Claim.Text, DriverDOB.Text);
            if (!validClaimAge)
            {
                MessageBox.Show("Driver cannot claim before he's of driving age.");
                return;
            }

            else if (CurrentDriverClaims.Contains(Convert.ToDateTime(Claim.Text)))
            {
                MessageBox.Show("You cannot specify more than 1 claim on a given date");
                return;
            }
            else if (CurrentDriverClaims.Count > 4)
            {
                MessageBox.Show("The Maximum number of claims for a given driver is 5"); ;
                return;
            }
            CurrentDriverClaims.Add(Convert.ToDateTime(Claim.Text));
            NoOfClaims.Visibility = System.Windows.Visibility.Visible;
            MessageBox.Show("Claim added to Driver with date " + Claim.Text);
            string currentClaimsLabel = "Current Driver Claims = " + CurrentDriverClaims.Count;
            NoOfClaims.Text = currentClaimsLabel;
            Claim.Text = "";
        }

        //Method for ensuring that Drivers are Older than 17 on a policy.
        private bool ValidateClaimsAge(string claimDate, string dobDate)
        {
            int AgeOfClaim = CalculateDriverPremiumAge(Convert.ToDateTime(dobDate), Convert.ToDateTime(claimDate));
            if (AgeOfClaim < 17)
            {
                return false;
            }
            return true;
        }

        //Method to calculate the overall premium for all driver's and policy information.
        private void CalculatePremium(object sender, MouseButtonEventArgs e)
        {
            int ClaimsCount = 0;
            NewPolicy.PolicyPrice = initialPremium;
            if (NewPolicy.PolicyStartDate == DateTime.MinValue)
            {
                MessageBox.Show("Please select an appropriate start date for your policy");
                return;
            }
            DateTime youngestDriverDOB = NewPolicy.Drivers[0].DateOfBirth, oldestDriverDOB = NewPolicy.Drivers[0].DateOfBirth;
            string youngestDriverName = NewPolicy.Drivers[0].Name, oldestDriverName =  NewPolicy.Drivers[0].Name;
            int totalClaims = 0;
            foreach(Driver driver in NewPolicy.Drivers){
                totalClaims += driver.Claims.Count;
            }
            foreach (Driver driver in NewPolicy.Drivers)
            {
                if (driver.Claims != null)
                {
                    ClaimsCount += driver.Claims.Count;
                    if (driver.Claims.Count > 2)
                    {
                        MessageBox.Show("Driver " + driver.Name + " has more than 2 claims.");
                        return;
                    }
                }
                  if (totalClaims > 3){
                      MessageBox.Show("Policy has more than 3 claims");
                      return;
                  }
                  if(driver.Occupation.Equals("Chauffeur")){
                      policyMultiplier += 0.1;
                  }
                  else{
                      policyMultiplier -= 0.1;
                  }
                  if (driver.DateOfBirth > NewPolicy.PolicyStartDate)
                  {
                      MessageBox.Show("Driver " + driver.Name + "must be born when Policy begins");
                      return;
                  }
                  if(driver.DateOfBirth > youngestDriverDOB){
                      youngestDriverDOB = driver.DateOfBirth;
                      youngestDriverName = driver.Name;
                  }
                  if(driver.DateOfBirth < oldestDriverDOB){
                      oldestDriverDOB = driver.DateOfBirth;
                      oldestDriverName = driver.Name;
                  }
            }
             int youngestDriverAge = CalculateDriverPremiumAge(youngestDriverDOB, NewPolicy.PolicyStartDate);
             int oldestDriverAge = CalculateDriverPremiumAge(oldestDriverDOB, NewPolicy.PolicyStartDate);
             if (youngestDriverAge < 21){
                 MessageBox.Show("Age of Youngest Driver: " + youngestDriverName);
                 return;
             }
             else if (oldestDriverAge > 75){
                  MessageBox.Show("Age of Oldest Driver: " + oldestDriverName);
                 return;
             }
             else if (youngestDriverAge >= 21 && youngestDriverAge < 25){
                 policyMultiplier += 0.2;
             }
             else if (youngestDriverAge >= 26 && youngestDriverAge < 75){
                 policyMultiplier -= 0.1;
             }
             NewPolicy.PolicyPrice = NewPolicy.PolicyPrice * policyMultiplier;
             MessageBox.Show("Premium for this Policy is £" + NewPolicy.PolicyPrice);
             policyMultiplier = 1;
            }
        
        //Method to perform calculations to determine age of driver at start of policy.
        private int CalculateDriverPremiumAge(DateTime driverBirthday, DateTime policyStartDate){
            int age = policyStartDate.Year - driverBirthday.Year;

            if(policyStartDate.Month < driverBirthday.Month || (policyStartDate.Month == driverBirthday.Month && policyStartDate.Day < driverBirthday.Day)){
                age--;
            }
            return age;
    }
        
        //Method to update Driver section of UI once a new driver has been added.
        private void ResetDriverUI()
        {
            Claim.Text = "";
            Claim.Visibility = System.Windows.Visibility.Hidden;
            CurrentDriverClaims = new List<DateTime>();
            DriversName.Text = "";
            DriverDOB.Text = "";
            Occupations.SelectedValue = -1;
            NoOfClaims.Text = "Current Driver Claims = " + CurrentDriverClaims.Count;
            Update_Claim.IsEnabled = false;
            P_Claim.IsEnabled = false;
            N_Claim.IsEnabled = false;
            Claim.Visibility = System.Windows.Visibility.Hidden;
        }

        //Method to remove the current Policy and allow a user to start fresh.
        private void DiscardPolicy(object sender, MouseButtonEventArgs e)
        {
            NewPolicy = new Policy();
            UpdateDriverInfos();
            PolicyDrivers.Text = "This policy currently has " + NewPolicy.Drivers.Count + " drivers.";
            CalcPremBtn.IsEnabled = false;
            CurrentDriverClaims = new List<DateTime>(); 
            Occupations.SelectedIndex = -1;
            DriversName.Text= "";
            DriverDOB.Text = "";
            Claim.Text = "";
            Claim.Visibility = System.Windows.Visibility.Hidden;
        }
        
        //Method to determine which driver has been selected to edit.
        private void SelectDriverToEdit(object sender, MouseButtonEventArgs e)
        {
            var mouseWasDownOn = e.Source as FrameworkElement;
            if (mouseWasDownOn != null)
            {
                EditingDriverId = Convert.ToInt32(mouseWasDownOn.Name[6].ToString()) - 1;
                EditDriver(Convert.ToInt32(mouseWasDownOn.Name[6].ToString())-1);
                
            }
        }

        //Method to update UI with selected driver's details and enable claim iteration.
        private void EditDriver(int driverId)
        {
            EditingDriversName = NewPolicy.Drivers[driverId].Name;
            Occupations.Text = NewPolicy.Drivers[driverId].Occupation;
            DriversName.Text = NewPolicy.Drivers[driverId].Name;
            DriverDOB.Text = NewPolicy.Drivers[driverId].DateOfBirth.ToString("dd/MM/yyyy");
            if (NewPolicy.Drivers[driverId].Claims.Count != 0)
            {
                Claim.Text = NewPolicy.Drivers[driverId].Claims[0].ToString("dd/MM/yyyy");
                CurrentDriverClaims = NewPolicy.Drivers[driverId].Claims;
                CurrentClaim = 0;
                NoOfClaims.Text = "Current Driver Claims = " + CurrentDriverClaims.Count;
                EditingClaim = Claim.Text;
                Update_Claim.IsEnabled = true;
                P_Claim.IsEnabled = true;
                N_Claim.IsEnabled = true;
            }
            ComfDriver.IsEnabled = false;
            Update_Driver.IsEnabled = true;
            Claim.Visibility = System.Windows.Visibility.Visible;
        }

        //Method to enable editing and updating information as opposed to adding a new driver
        private void UpdateDriverDetails(object sender, MouseButtonEventArgs e)
        {
            EditMode = true;
            ConfirmDriver(e, e);
            ComfDriver.IsEnabled = true;
            Update_Driver.IsEnabled = false;
        }

        //Method to allow current Driver's claim dates to be changed and updated.
        private void EditClaim(object sender, MouseButtonEventArgs e)
        {
                for (int i = 0; i < NewPolicy.Drivers.Count; i++)
                {
                    if (NewPolicy.Drivers[i].Name == EditingDriversName)
                    {
                        for (int j = 0; j < NewPolicy.Drivers[i].Claims.Count; j++)
                            if (Convert.ToString(NewPolicy.Drivers[i].Claims[j].ToString("dd/MM/yyyy")) == EditingClaim)
                            {
                                NewPolicy.Drivers[i].Claims[j] = Convert.ToDateTime(Claim.Text);
                                MessageBox.Show("Claim date updated");
                            }
                    }
                }
        }
        //Method to iteration forwards through a Driver's claims.
        private void NextClaim(object sender, MouseButtonEventArgs e)
        {
            if(CurrentClaim < CurrentDriverClaims.Count-1){
                 CurrentClaim++;
                 Claim.Text = CurrentDriverClaims[CurrentClaim].ToString("dd/MM/yyyy");
                 EditingClaim = Claim.Text;
            }
            else{
                MessageBox.Show("Already displaying last claim");
            }
        }

        //Method to iteration backwards through a Driver's claims.
        private void PrevClaim(object sender, MouseButtonEventArgs e)
        {
            if (CurrentClaim > 0)
            {
                CurrentClaim--;
                Claim.Text = CurrentDriverClaims[CurrentClaim].ToString("dd/MM/yyyy");
                EditingClaim = Claim.Text;
            }
            else
            {
                MessageBox.Show("Already displaying first claim");
            }
        }
    }
}
