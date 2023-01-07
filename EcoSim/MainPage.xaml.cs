using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace EcoSim
{
    public abstract class LifeForm
    {
        public int Health { get; set; }
        public int Energy { get; set; }
        public bool IsMobile { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int VisionRadius { get; set; }
        public int ContactRadius { get; set; }
        public bool IsAlive { get; set; }


        public LifeForm(int health, int energy, bool isMobile, int x, int y, int visionRadius, int contactRadius)
        {
            Health = health;
            Energy = energy;
            IsMobile = isMobile;
            X = x;
            Y = y;
            VisionRadius = visionRadius;
            ContactRadius = contactRadius;
            IsAlive = true;
        }

        public virtual void Update() 
        {
            Energy--;
            if (Energy <= 0)
            {
                ConvertHealthToEnergy();
            }
            if (Health <= 0)
            {
                Die();
            }
        }
        public void ConvertHealthToEnergy()
        {
            Energy += Health;
            Health = 0;
        }
        public abstract void Feed(LifeForm food);
        public abstract void Reproduce();
        public virtual void Die()
        {
            // Actions de mise à mort de la forme de vie, si nécessaire
            if (EcoSim.LifeForms.Contains(this))
            {
                EcoSim.LifeForms.Remove(this);
            }
        }
    }
    public class Animal : LifeForm
    {
        public int Age { get; set; }
        public bool IsCarnivore { get; set; }
        public bool IsMale { get; set; }
        public static readonly int REPRODUCTION_AGE = 3;
        public LifeForm Partner { get; set; }

        public Animal(int age, int health, int energy, bool isMobile, int x, int y, int visionRadius, int contactRadius, bool isCarnivore, bool isMale)
            : base(health, energy, isMobile, x, y, visionRadius, contactRadius)
        {
            IsCarnivore = isCarnivore;
            IsMale = isMale;
            Age = age;
        }
   
        public override void Update()
        {
            base.Update();

            FindPartner();

            LeaveOrganicWaste();

            Age++;

            if(Age >= REPRODUCTION_AGE && Partner != null)
            {
                Reproduce();
            }

            // Recherche de nourriture dans la zone de vision
            List<LifeForm> nearbyFood = new List<LifeForm>();
            if (!IsCarnivore)
            {
                nearbyFood = SearchForPlants();
            }
            if (nearbyFood.Count > 0)
            {
                // Si de la nourriture est trouvée, se nourrit
                Feed(nearbyFood[0]);
            }
            // Si l'animal est mobile, il se déplace
            if (IsMobile)
            {
                Move();
            }
        }

        public override void Feed(LifeForm food)
        {
            // Vérifie si l'animal est herbivore ou carnivore
            if (IsCarnivore)
            {
                // Si l'animal est carnivore, il peut se nourrir de toutes les formes de vie sauf de plantes
                if (!(food is Plant) && !(food is OrganicWaste))
                {
                    // L'animal se nourrit de la nourriture
                    Energy += food.Energy;
                    food.Die();
                }
            }
            else
            {
                // Si l'animal est herbivore, il peut se nourrir de plantes uniquement
                if (food is Plant)
                {
                    // L'animal se nourrit de la nourriture
                    Energy += food.Energy;
                    food.Die();
                }
            }
        }

        public void Move()
        {
            // Génère un mouvement aléatoire
            Random random = new Random();
            int dx = random.Next(-1, 2);
            int dy = random.Next(-1, 2);

            // Déplace l'animal d'une case dans la direction choisie
            X += dx;
            Y += dy;
        }

        public List<LifeForm> SearchForFood()
        {
            // Recherche de nourriture dans la zone de vision
            List<LifeForm> nearbyFood = new List<LifeForm>();
            foreach (LifeForm lifeForm in EcoSim.LifeForms)
            {
                if (lifeForm.IsAlive && IsInVisionRange(lifeForm))
                {
                    nearbyFood.Add(lifeForm);
                }
            }
            return nearbyFood;
        }

        public List<LifeForm> SearchForPreys()
        {
            // Recherche de proies dans la zone de vision
            List<LifeForm> nearbyPreys = new List<LifeForm>();
            foreach (LifeForm lifeForm in EcoSim.LifeForms)
            {
                if (lifeForm.IsAlive && lifeForm is Animal && IsInVisionRange(lifeForm))
                {
                    nearbyPreys.Add(lifeForm);
                }
            }
            if (nearbyPreys.Count > 0 && IsCarnivore)
            {
                // Si une proie est trouvée, l'animal carnivore l'attaque
                Attack(nearbyPreys[0]);
            }
            if (nearbyPreys.Count == 0 && IsCarnivore) ;
            {
                // Recherche de nourriture dans la zone de vision
                List<LifeForm> nearbyFood = SearchForFood();
                if (nearbyFood.Count > 0)
                {
                    // Si de la nourriture est trouvée, se nourrit
                    Feed(nearbyFood[0]);
                }
            }
            return nearbyPreys;
        }

        public List<LifeForm> SearchForPlants()
        {
            // Recherche de plantes dans la zone de vision
            List<LifeForm> nearbyPlants = new List<LifeForm>();
            foreach (LifeForm lifeForm in EcoSim.LifeForms)
            {
                if (lifeForm is Plant && IsInVisionRange(lifeForm))
                {
                    nearbyPlants.Add(lifeForm);
                }
            }
            return nearbyPlants;
        }

        public bool IsInVisionRange(LifeForm lifeForm)
        {
            // Vérifie si la forme de vie est dans la zone de vision de l'animal
            int dx = lifeForm.X - X;
            int dy = lifeForm.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= VisionRadius;
        }

        public void FindPartner()
        {
            // Recherche d'un partenaire dans la zone de contact
            foreach (LifeForm lifeForm in EcoSim.LifeForms)
            {
                if (lifeForm.IsAlive && IsInContactRange(lifeForm) && Partner == null)
                {
                    Partner = lifeForm;
                    break;
                }
            }
        }

        public bool IsInContactRange(LifeForm lifeForm)
        {
            // Vérifie si la forme de vie est dans la zone de contact de l'animal
            int dx = lifeForm.X - X;
            int dy = lifeForm.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= ContactRadius;
        }

        public override void Reproduce()
        {
            // Vérifie si l'animal et son partenaire sont de sexes différents et si l'animal est en âge de se reproduire
            if (Partner is Animal && IsMale != ((Animal)Partner).IsMale && Age >= REPRODUCTION_AGE)
            {
                // Crée une instance de la classe Random
                Random random = new Random();

                // Génère aléatoirement le sexe du petit
                bool randomSex = random.Next(0, 2) == 1;

                // Crée un nouvel animal qui représente le petit
                Animal baby = new Animal(age: 0, health: 100, energy: 50, isMobile: true, x: X + 1, y: Y + 1, visionRadius: 5, contactRadius: 1, isCarnivore: IsCarnivore, isMale: randomSex);

                // Place le petit à proximité de la femelle (c'est-à-dire à côté de l'animal qui appelle la méthode)
                baby.X = X + 1;
                baby.Y = Y + 1;
            }
        }

        public void Attack(LifeForm prey)
        {
            // Vérifie si l'animal est en zone de contact avec sa proie et si la proie est un animal
            if (IsInContactRange(prey) && prey is Animal)
            {
                // L'animal attaque sa proie et la tue
                prey.Health = 0;
                prey.Die();
            }
        }

        private void LeaveOrganicWaste()
        {
            // Générer un nombre aléatoire entre 0 et 100
            Random random = new Random();
            int randomNumber = random.Next(0, 101);

            // Si le nombre aléatoire est inférieur à 10, laisser un déchet organique
            if (randomNumber < 10)
            {
                OrganicWaste organicWaste = new OrganicWaste(Energy / 2, X, Y);
                EcoSim.LifeForms.Add(organicWaste);
            }
        }

        public override void Die()
        {
            IsAlive = false;
            // L'animal devient de la viande
            Meat meat = new Meat(Energy, X, Y);
            EcoSim.LifeForms.Add(meat);
        }
    }    
    public class OrganicWaste : LifeForm
    {
        public OrganicWaste(int energy, int x, int y)
            : base(0, energy, false, x, y, 0, 0)
        {
        }

        public override void Update()
        {
            // Les déchets organiques perdent de l'énergie avec le temps
            Energy--;
            if (Energy <= 0)
            {
                Die();
            }
        }

        public override void Feed(LifeForm food)
        {
            // Les déchets organiques ne peuvent pas se nourrir
        }

        public override void Reproduce()
        {
            // Les déchets organiques ne peuvent pas se reproduire
        }

        public override void Die()
        {
            // Les déchets organiques disparaissent lorsqu'ils n'ont plus d'énergie
            IsAlive = false;
            EcoSim.LifeForms.Remove(this);
        }
    }
    public class Meat : LifeForm
    {
        public Meat(int energy, int x, int y)
            : base(0, energy, false, x, y, 0, 0)
        {
        }

        public override void Update()
        {
            // La viande perd de l'énergie avec le temps
            Energy--;
            if (Energy <= 0)
            {
                Die();
            }
        }

        public override void Feed(LifeForm food)
        {
            // La viande ne peut pas se nourrir
        }

        public override void Reproduce()
        {
            // La viande ne peut pas se reproduire
        }

        public override void Die()
        {
            // La viande devient un déchet organique lorsqu'elle meurt
            IsAlive = false;
            OrganicWaste organicWaste = new OrganicWaste(Energy, X, Y);
            EcoSim.LifeForms.Add(organicWaste);
        }
    }
    public class Plant : LifeForm
    {
        public int RootRadius { get; set; }
        public int SowingRadius { get; set; }

        public Plant(int health, int energy, bool isMobile, int x, int y, int visionRadius, int contactRadius, int rootRadius, int sowingRadius)
            : base(health, energy, isMobile, x, y, visionRadius, contactRadius)
        {
            RootRadius = rootRadius;
            SowingRadius = sowingRadius;
        }
        
        public override void Update()
        {
            base.Update();

            // Recherche de déchets organiques dans la zone de racines
            List<OrganicWaste> nearbyOrganicWaste = SearchForOrganicWaste();
            if (nearbyOrganicWaste.Count > 0)
            {
                // Si des déchets organiques sont trouvés, se nourrit
                Feed(nearbyOrganicWaste[0]);
                nearbyOrganicWaste.Remove(nearbyOrganicWaste[0]);
            }

            // Répand des graines dans la zone de semis
            SpreadSeeds();
        }
        private List<OrganicWaste> SearchForOrganicWaste()
        {
            // Recherche des déchets organiques dans la zone de racines
            List<OrganicWaste> nearbyOrganicWaste = new List<OrganicWaste>();
            foreach (LifeForm lifeForm in EcoSim.LifeForms)
            {
                if (lifeForm is OrganicWaste && IsInRootRange(lifeForm))
                {
                    nearbyOrganicWaste.Add((OrganicWaste)lifeForm);
                }
            }
            return nearbyOrganicWaste;
        }

        private bool IsInRootRange(LifeForm lifeForm)
        {
            // Vérifie si la forme de vie est dans la zone de racines de la plante
            int dx = lifeForm.X - X;
            int dy = lifeForm.Y - Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= RootRadius;
        }

        public void SpreadSeeds()
        {
            // Répand des graines dans la zone de semis
            Random random = new Random();
            int dx = random.Next(-SowingRadius, SowingRadius + 1);
            int dy = random.Next(-SowingRadius, SowingRadius + 1);
            int newX = X + dx;
            int newY = Y + dy;
            Plant newPlant = new Plant(Health, Energy, false, newX, newY, VisionRadius, ContactRadius, RootRadius, SowingRadius);
            EcoSim.LifeForms.Add(newPlant);
            Energy -= 10;
        }

        public override void Feed(LifeForm food)
        {
            // La plante se nourrit en consommant des déchets organiques dans sa zone de racines
            if (food is OrganicWaste)
            {
                Energy += food.Energy;
                food.Die();
            }
        }

        public override void Reproduce()
        {
            // De nouvelles plantes apparaissent dans la zone de semis autour de la plante existante
            SpreadSeeds();
        }

        public override void Die()
        {
            IsAlive = false;
            // La plante devient un déchet organique
            EcoSim.LifeForms.Add(new OrganicWaste(Energy, X, Y));
        }
    }
    public class EcoSim
    {
        public static List<LifeForm> LifeForms { get; set; }
        public static List<int> Debris { get; set; }

        public void Update()
        {
            // Mise à jour de toutes les formes de vie
            foreach (var lifeForm in LifeForms)
            {
                lifeForm.Update();
            }

            // Mise à jour des déchets organiques (pourriture, etc.)
            for (int i = 0; i < Debris.Count; i++)
            {
                Debris[i]--;
                if (Debris[i] <= 0)
                {
                    Debris.RemoveAt(i);
                    i--;
                }
            }
        }
    }
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void OnAddAnimalClicked(object sender, EventArgs e)
        {
            // Récupération des valeurs saisies par l'utilisateur
            int age = int.Parse(AgeEntry.Text);
            int health = int.Parse(HealthEntry.Text);
            int energy = int.Parse(EnergyEntry.Text);
            bool isMobile = IsMobileSwitch.IsToggled;
            int x = int.Parse(XEntry.Text);
            int y = int.Parse(YEntry.Text);
            int visionRadius = int.Parse(VisionRadiusEntry.Text);
            int contactRadius = int.Parse(ContactRadiusEntry.Text);
            bool isCarnivore = IsCarnivoreSwitch.IsToggled;
            bool isMale = IsMaleSwitch.IsToggled;

            // Création d'un nouvel animal avec les paramètres souhaités
            LifeForm newLifeForm = new Animal(age, health, energy, isMobile, x, y, visionRadius, contactRadius, isCarnivore, isMale);

            // Ajout de l'animal à la liste des formes de vie
            EcoSim.LifeForms.Add(newLifeForm);
        }
        private void OnAddPlantClicked(object sender, EventArgs e)
        {
            // Récupération des valeurs saisies par l'utilisateur
            int health = int.Parse(HealthEntry.Text);
            int energy = int.Parse(EnergyEntry.Text);
            bool isMobile = IsMobileSwitch.IsToggled;
            int x = int.Parse(XEntry.Text);
            int y = int.Parse(YEntry.Text);
            int visionRadius = int.Parse(VisionRadiusEntry.Text);
            int contactRadius = int.Parse(ContactRadiusEntry.Text);
            int rootRadius = int.Parse(RootRadiusEntry.Text);
            int sowingRadius = int.Parse(SowingRadiusEntry.Text);

            // Création d'une nouvelle plante avec les paramètres souhaités
            LifeForm newLifeForm = new Plant(health, energy, isMobile, x, y, visionRadius, contactRadius, rootRadius, sowingRadius);

            // Ajout de l'animal à la liste des formes de vie
            EcoSim.LifeForms.Add(newLifeForm);
        }

    }
}