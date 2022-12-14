using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;


namespace ChasseAuTresor
{
    class Program
    {
        //création du dictionnaire pour éviter erreurs de frappes lors d'indication de lignes / colonnes
        static char[] dico = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

        static void Main(string[] args)
        {
            Console.SetWindowSize(140, 30);
            Run();
        }


        // Sous programmes -------------------------------------------------------------------------------------

        // ===== INITIALISATION ===== //

        static void Run()
        // Sert de fonction Main pouvant être appelée pour recommencer.
        {

            Random rng = new Random(); // définir la variable random
            AfficherTitre();
            Console.WriteLine();
            int compteurTours = 1;
            string[,] grille = Initialiser(rng); //initialise la grille de jeu
            Console.WriteLine($" Tour n°{compteurTours}");
            int[,] verification = InitialiserVerif(grille); //Initialise une grille utile à la fonction Recursivité (case déjà vérifiée)

            int ligne;
            int colonne;

            string ansLigne;
            string ansCol;
            do
            {
                Console.WriteLine(); Console.Write(@" Choisissez le numéro de ligne : ");
                ansLigne = Console.ReadLine();
                GererMissClick(ref ansLigne, "ligne");

                Console.Write(@" Choisissez le numéro de colonne : ");
                ansCol = Console.ReadLine();
                GererMissClick(ref ansCol, "colonne");

                ligne = int.Parse(ansLigne) - 1;
                colonne = int.Parse(ansCol) - 1;
            } while (ligne < 0 || ligne >= grille.GetLength(0) || colonne < 0 || colonne >= grille.GetLength(1));



            int[,] positionsMines = PlacerMines(ligne, colonne, grille, rng);                                        // "Placement" des Mines
            int[,] positionsTresors = PlacerTresors(ligne, colonne, grille, positionsMines, rng);                   // "Placement" des trésors
            Jouer(grille, positionsMines, positionsTresors, ref compteurTours, ligne, colonne, verification); // La chasse au trésor commence avec les coordonnées indiquées
        }


        static string[,] Initialiser(Random rng)
        /* Demande taille du plateau au joueur 
         * et crée le tableau de jeu */
        {
            Console.WriteLine("                 Veuillez indiquez les dimensions de la grille ");
            Console.Write("                 Combien de lignes voulez-vous ? ");
            string ans = Console.ReadLine();
            GererMissClick(ref ans, "lignes");

            int lignesGrille = int.Parse(ans);

            Console.Write("                 Combien de colonnes voulez-vous ? ");
            ans = Console.ReadLine();
            GererMissClick(ref ans, "colonnes");


            int colonnesGrille = int.Parse(ans);


            string[,] grille = new string[lignesGrille, colonnesGrille];

            for (int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    grille[i, j] = " ND ";  // Contenu des cases non découvertes
                }
            }
            Console.WriteLine();

            AnimerDepartJeu(rng);

            Console.Clear();

            AfficherGrille(grille); // demande affichage de la grille
            return grille;
        }

        static int[,] InitialiserVerif(string[,] grille)
        /* Demande taille du plateau au joueur 
         * et crée le tableau de la grille "vérification" */
        {
            int[,] verification = new int[grille.GetLength(0), grille.GetLength(1)];
            for (int i = 0; i < verification.GetLength(0); i++)
            {
                for (int j = 0; j < verification.GetLength(1); j++)
                {
                    verification[i, j] = 0;
                }
            }
            return verification;
        }


        static int[,] PlacerMines(int ligne, int colonne, string[,] grille, Random rng)
        // Place les mines 
        {
            //int nbMines = rng.Next(grille.GetLength(0) / 2, grille.GetLength(0) * grille.GetLength(1) / 2 + 1); // + 1 car on veut la moitié pile du tableau et pas la moitié - 1
            int nbMines = 3;
            int[,] positionsMines = new int[nbMines, 2];
            /* matrice des positions des mines 
             * |x1,y1| 1ere mine
             * |x2,y2| 2ème mine etc*/

            int indice = 0;
            for (int i = 0; i < positionsMines.GetLength(0); i++)
            {
                //créer les coordonnées d'UNE MINE
                indice = i;
                int x = rng.Next(0, grille.GetLength(0));
                int y = rng.Next(0, grille.GetLength(1));
                for (int j = indice; j >= 0; j--)
                {
                    while (((positionsMines[j, 0] == x) && (positionsMines[j, 1] == y)) || ((x == ligne) && (y == colonne)))
                    {
                        x = rng.Next(0, grille.GetLength(0));
                        y = rng.Next(0, grille.GetLength(1));
                    }
                    // Fin de la création de coordonnées

                    // Début positionnement
                    positionsMines[i, 0] = x; // coordonnées x des mines
                    positionsMines[i, 1] = y; // coordonnées y des mines
                }
            }
            return positionsMines;
        }


        static int[,] PlacerTresors(int ligne, int colonne, string[,] grille, int[,] positionsMines, Random rng)
        // Place les trésors
        {
            bool test = false;

            int nbTresors = rng.Next(1, 4); // Entre 1 et 3 trésors
            int[,] positionsTresors = new int[nbTresors, 2];
            /* matrice des positions des trésors 
             * |x1,y1| 1er trésor
             * |x2,y2| 2ème trésor etc*/

            for (int i = 0; i < positionsTresors.GetLength(0); i++)//Remplissage de positions Trésor
            {
                test = false;
                int x = 0;
                int y = 0;
                while (test == false) //On change les coordonnées du trésor si les conditions ne sont pas respectées
                {
                    test = true;
                    x = rng.Next(0, grille.GetLength(0));
                    y = rng.Next(0, grille.GetLength(1));

                    if (x == ligne && colonne == y) // Si le trésor est placé sur le 1er coup
                    {
                        test = false;
                    }
                    else
                    {
                        if (CheckerPositions(x, y, positionsMines) == true) // si le trésor est sur une mine
                        {
                            test = false;
                        }
                        else
                        {
                            for (int o = 0; o < positionsTresors.GetLength(0); o++) //parcourt les positions des trésors
                            {
                                if (x == positionsTresors[o, 0] && y == positionsTresors[o, 1]) // si ce n'est pas un trésor déjà existant on rentre
                                {
                                    test = false;
                                }
                            }
                        }
                    }
                }
                positionsTresors[i, 0] = x; // coordonnées x des trésors
                positionsTresors[i, 1] = y; // coordonnées y des trésors

            }
            return positionsTresors;
        }


        // ====== JEU ===== //

        static void Jouer(string[,] grille, int[,] positionsMines, int[,] positionsTresors, ref int compteurTours, int ligne, int colonne, int[,] verification)
        // Demande au joueur de jouer un coup et le joue
        {
            bool lost = EtrePerdue(ligne, colonne, grille, positionsMines);
            bool win = EtreGagnee(positionsMines, grille);

            if (compteurTours != 1 && win != true && lost == false)
            {
                do
                {
                    Console.WriteLine();
                    Console.Write(" Choisissez le numéro de ligne : ");
                    string ans = Console.ReadLine();
                    GererMissClick(ref ans, "ligne");
                    ligne = int.Parse(ans) - 1;
                    Console.Write(" Choisissez le numéro de colonne : ");
                    ans = Console.ReadLine();
                    GererMissClick(ref ans, "colonne");
                    colonne = int.Parse(ans) - 1;
                } while (ligne < 0 || ligne >= grille.GetLength(0) || colonne < 0 || colonne >= grille.GetLength(1));
            }

            Recursivite(grille, positionsMines, positionsTresors, ref compteurTours, ligne, colonne, verification);
            lost = EtrePerdue(ligne, colonne, grille, positionsMines);
            win = EtreGagnee(positionsMines, grille);

            // Récupère des indicateurs de partie [Gagnée/Perdue]
            if (lost == true)
            {
                AnimerGameOver(compteurTours, grille);
            }
            else
            {
                if (win == true)
                {
                    AnimerVictoire(compteurTours);
                }
                else
                {
                    if (win == false && lost == false) // ni victoire ni défaite, la partie continue.
                    {
                        compteurTours += 1;
                        Console.Clear();
                        //Instruction du jeu
                        AfficherGrille(grille);
                        Console.WriteLine($" Tour n°{compteurTours}");

                        //Relancez le jeu tant que la partie n'est pas finie
                        Jouer(grille, positionsMines, positionsTresors, ref compteurTours, ligne, colonne, verification);
                    }
                }
            }
        }


        // ===== FONCTIONNALITES ===== //

        static bool GererMissClick(ref string ans, string rangee)
        // Gestion des potentielles erreurs de frappe lors d'indication des lignes/colonnes,
        // redemande tant que ce ne sont pas des chiffres/nombres
        {
            bool test = false;
            int i = 0;
            if (ans == "") // si la personne appuie sur Entrée sans indiquer de valeur
            {
                if (rangee == "lignes" || rangee == "colonnes")
                {
                    Console.Write($"                 Combien de {rangee} voulez-vous ? ");
                    ans = Console.ReadLine();
                    GererMissClick(ref ans, rangee);
                }
                else
                {
                    Console.Write($" Choisissez le numéro de {rangee} : ");
                    ans = Console.ReadLine();
                    GererMissClick(ref ans, rangee);
                }

            }
            else // une valeur a été indiquée, vérification que c'est un nombre/chiffre
            {

                while (i < ans.Length)
                {
                    int j = 0;
                    bool contient = false;
                    while (j < dico.Length && contient == false)
                    {
                        if (ans[i] == dico[j])
                        {
                            contient = true;
                        }
                        j++;
                    }
                    if (contient == false && (rangee == "lignes" || rangee == "colonnes"))
                    {
                        Console.Write($"                 Combien de {rangee} voulez-vous ? ");
                        ans = Console.ReadLine();
                        GererMissClick(ref ans, rangee);
                    }
                    else
                    {
                        if (contient == false )
                        { 
                            Console.Write($" Choisissez le numéro de {rangee} : ");
                            ans = Console.ReadLine();
                            GererMissClick(ref ans, rangee);
                        }
                    }
                    i++;
                }
            }
            test = true;
            return test;
        }

        static void AfficherGrille(string[,] tab)
        //Affiche la grille de jeu
        {
            Console.WriteLine();
            Console.Write("        1");
            for (int l = 1; l < tab.GetLength(1); l++)
            {
                Console.Write($"     {l + 1}");
            }
            Console.Write("   ");
            Console.WriteLine();
            Console.Write("       ");
            string trait = new string('—', 4);
            for (int l = 0; l < tab.GetLength(1); l++)
            {
                Console.Write(trait + "  ");
            }
            Console.WriteLine();
            for (int i = 0; i < tab.GetLength(0); i++)
            {
                Console.Write("  ");
                if (i >= 9)
                {
                    Console.Write($"{i + 1} |");
                }
                else { Console.Write($"{i + 1}  |"); }

                for (int j = 0; j < tab.GetLength(1); j++)
                {
                    if (tab[i, j] == "  T ")
                    {
                        Console.Write(" ");
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                    }
                    else
                    {
                        if (tab[i, j] == "  M ")
                        {
                            Console.Write(" ");
                            Console.BackgroundColor = ConsoleColor.DarkRed;
                        }
                        else
                        {
                            if (tab[i, j] == "    ")
                            {
                                Console.Write(" ");
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                            }
                            else
                            {
                                if (tab[i, j] == " ND ")
                                {
                                    Console.Write(" ");
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else
                                {
                                    if (tab[i, j] == " 1  ")
                                    {
                                        Console.Write(" ");
                                        Console.BackgroundColor = ConsoleColor.DarkGray;
                                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                                    }
                                    else
                                    {
                                        if (tab[i, j] == " 2  ")
                                        {
                                            Console.Write(" ");
                                            Console.BackgroundColor = ConsoleColor.DarkGray;
                                            Console.ForegroundColor = ConsoleColor.Green;
                                        }
                                        else
                                        {
                                            Console.Write(" ");
                                            Console.BackgroundColor = ConsoleColor.DarkGray;
                                            Console.ForegroundColor = ConsoleColor.DarkRed;
                                        }
                                    }

                                }
                            }
                        }
                    }
                    Console.Write(tab[i, j]);
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(" ");
                }
                Console.Write(" ");
                Console.WriteLine();
                for (int a = 0; a < 4 * tab.GetLength(1) + 7 + 2 * (tab.GetLength(1) - 1) + 1; a++)
                {
                    Console.Write(" ");
                }

                Console.WriteLine(" ");
            }
        }


        static bool CheckerPositions(int i, int j, int[,] positionsItems)
        // renvoie true si la position vérifiée est une mine ou un trésor, false sinon
        {
            bool reponse = false;
            for (int indice = 0; indice < positionsItems.GetLength(0); indice++)
            {
                if (positionsItems[indice, 0] == i && positionsItems[indice, 1] == j)
                {
                    reponse = true;
                    return reponse;
                }
            }
            return reponse;
        }


        static int CheckerEntourage(int ligne, int colonne, int[,] positionsMines, int[,] positionsTresors, string[,] grille)
        // affiche le nombre associé à la case
        {
            int indiceLigneDep = ligne - 1;
            int indiceLigneFin = ligne + 1;
            int indiceColonneDep = colonne - 1;
            int indiceColonneFin = colonne + 1;
            int compteur = 0;
            if (ligne == 0)
            {
                indiceLigneDep += 1;
            }
            if (colonne == 0)
            {
                indiceColonneDep += 1;
            }
            if (ligne == grille.GetLength(0) - 1)
            {
                indiceLigneFin -= 1;
            }
            if (colonne == grille.GetLength(1) - 1)
            {
                indiceColonneFin -= 1;
            }
            for (int i = indiceLigneDep; i <= indiceLigneFin; i++)
            {
                for (int j = indiceColonneDep; j <= indiceColonneFin; j++)
                {
                    if (CheckerPositions(i, j, positionsMines))
                    {
                        compteur += 1;
                    }
                    if (CheckerPositions(i, j, positionsTresors))
                    {
                        compteur += 2;
                    }
                }
            }
            return compteur;
        }

        static void Recursivite(string[,] grille, int[,] positionsMines, int[,] positionsTresors, ref int compteurTours, int ligne, int colonne, int[,] verification)
        {
            // Fonction récursive qui révèle les cases non indicées, sinon indique le numéro calculée sur la case
            if (verification[ligne, colonne] == 0) //case non decouverte
            {
                verification[ligne, colonne] = 1;
                if (CheckerEntourage(ligne, colonne, positionsMines, positionsTresors, grille) == 0)
                {
                    //On parcourt les 8 cases autour de la case jouée
                    int indiceLigneDep = ligne - 1;
                    int indiceLigneFin = ligne + 1;
                    int indiceColonneDep = colonne - 1;
                    int indiceColonneFin = colonne + 1;
                    // Vérification des IndexOutOfRange, sur les bords
                    if (ligne == 0)
                    {
                        indiceLigneDep = 0;
                    }
                    else
                    {
                        if (ligne == grille.GetLength(0) - 1)
                        {
                            indiceLigneFin -= 1;
                        }
                    }
                    if (colonne == 0)
                    {
                        indiceColonneDep = 0;
                    }
                    else
                    {
                        if (colonne == grille.GetLength(1) - 1)
                        {
                            indiceColonneFin -= 1;
                        }
                    }

                    for (int i = indiceLigneDep; i <= indiceLigneFin; i++)
                    {
                        for (int j = indiceColonneDep; j <= indiceColonneFin; j++)
                        {
                            if (CheckerEntourage(i, j, positionsMines, positionsTresors, grille) == 0)
                            {
                                grille[i, j] = "    ";        //vider la case si elle est sans chiffre
                                if (i != 0 || j != 0)
                                {
                                    Recursivite(grille, positionsMines, positionsTresors, ref compteurTours, i, j, verification);
                                }
                            }
                            else
                            {
                                int compteur = (CheckerEntourage(i, j, positionsMines, positionsTresors, grille));
                                grille[i, j] = " " + compteur + "  "; // Affiche le numéro calculée sur la case si il y a des trésor ou mines autour
                                verification[i, j] = 1;
                            }
                        }
                    }
                    grille[ligne, colonne] = "    ";
                    verification[ligne, colonne] = 1;

                }
                else
                {
                    // Si il y a un item autour, afficher le numéro calculée directement sur la case
                    if (CheckerPositions(ligne, colonne, positionsTresors))
                    {
                        System.Media.SoundPlayer tresorTrouve = new System.Media.SoundPlayer(Properties.Resources.treasureOpening);
                        tresorTrouve.Play();
                        grille[ligne, colonne] = "  T ";
                    }
                    else
                    {
                        grille[ligne, colonne] = " " + CheckerEntourage(ligne, colonne, positionsMines, positionsTresors, grille) + "  ";
                    }
                    verification[ligne, colonne] = 1;
                }
                Console.WriteLine();
            }
        }


        static bool EtrePerdue(int ligne, int colonne, string[,] grille, int[,] positionsMines)
        // Détecte si le joueur a touché une mine et arrête la partie ou continue la partie sinon
        {

            bool lost = false;
            for (int i = 0; i < positionsMines.GetLength(0); i++)
            {
                if (positionsMines[i, 0] == ligne && positionsMines[i, 1] == colonne)
                {
                    lost = true;
                    Console.Clear();
                    grille[ligne, colonne] = "  M ";
                    AfficherGrille(grille);
                }
            }
            return lost;
        }


        static bool EtreGagnee(int[,] positionsMines, string[,] grille)
        // Détecte si le joueur a gagné et arrête la partie
        {
            bool win = false;
            for (int i = 0; i < grille.GetLength(0); i++)
            {
                for (int j = 0; j < grille.GetLength(1); j++)
                {
                    // Si une case non définie n'est pas une mine, la partie n'est pas gagnée ni terminée
                    if (grille[i, j] == " ND " && CheckerPositions(i, j, positionsMines) == false)
                    {
                        return win;
                    }
                }
            }
            win = true;
            Console.Clear();
            AfficherGrille(grille);
            return win;
        }


        // ===== COSMETIQUE ===== //

        static void AnimerDepartJeu(Random rng)
        //petite animation d'écriture avec fausse erreur pouvant apparaître
        {
            Console.Write(@"


                                        ");
            string phrase = @"Création de la grille de jeu";
            System.Media.SoundPlayer playerTyping = new System.Media.SoundPlayer(Properties.Resources.typing); // création variable pour le son, en cherchant le .wav dans les Resources
            playerTyping.Play();
            // Animation du texte
            for (int i = 0; i < phrase.Length; i++)
            {
                Console.Write(phrase[i]);
                System.Threading.Thread.Sleep(50);
            }
            // Trois points de suspension ...
            System.Threading.Thread.Sleep(1000);
            Console.Write(".");
            System.Threading.Thread.Sleep(1000);
            Console.Write(".");
            System.Threading.Thread.Sleep(1000);
            Console.Write(".");
            System.Threading.Thread.Sleep(1000);

            int reboot = rng.Next(0, 6);
            if (reboot == 0) // 1 chance sur 5 de recevoir le message d'erreur.
            {
                for (int i = 0; i < phrase.Length + 3; i++)
                {
                    Console.Write("\b");
                }
                playerTyping.Play();
                string erreur = @"Erreur de génération du plateau.";
                for (int i = 0; i < erreur.Length; i++)
                {
                    Console.Write(erreur[i]);
                    System.Threading.Thread.Sleep(50);
                }
                System.Threading.Thread.Sleep(1000);
                playerTyping.Play();
                string nouvelleTentative = @" Nouvelle tentative de création...";
                for (int i = 0; i < nouvelleTentative.Length; i++)
                {
                    Console.Write(nouvelleTentative[i]);
                    System.Threading.Thread.Sleep(50);
                }
                System.Threading.Thread.Sleep(1000);
            }
        }
        static void AnimerGameOver(int compteurTours, string[,] grille)
        // Affiche la page de fin de la partie, lorsque perdue.
        {
            Console.WriteLine();
            System.Media.SoundPlayer gameOver = new System.Media.SoundPlayer(Properties.Resources.explosion); // son d'explosion à la perte de la partie
            gameOver.Play();

            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Clear(); //Colore la console en Rouge

            string centrer = new string(' ', Console.WindowHeight / 2); //variable pour centrer le texte;
            // Affiche le message "Game Over"
            Console.Write(centrer);
            Console.WriteLine(@"                                  )                    ");
            Console.Write(centrer);
            Console.WriteLine(@"  (                            ( /(                    ");
            Console.Write(centrer);
            Console.WriteLine(@"   )\ )       )     )      (    )\())   )      (   (    ");
            Console.Write(centrer);
            Console.WriteLine(@"  (()/(    ( /(    (      ))\  ((_)\   /((    ))\  )(   ");
            Console.Write(centrer);
            Console.WriteLine(@" /(_))_  )(_))   )\  ' /((_)   ((_) (_))\  /((_)(()\  ");
            Console.Write(centrer);
            Console.WriteLine(@"(_)) __|((_)_  _((_)) (_))    / _ \ _)((_)(_))   ((_) ");
            Console.Write(centrer);
            Console.WriteLine(@"  | (_ |/ _` || '  \()/ -_)  | (_) |\ V / / -_) | '_| ");
            Console.Write(centrer);
            Console.WriteLine(@"   \___|\__,_||_|_|_| \___|   \___/  \_/  \___| |_|   ");
            Console.WriteLine();

            Console.Write(centrer);
            Console.WriteLine($"Tu as perdu en {compteurTours} tours.");
            // La partie est finie car la bombe a explosé 
            Console.Write(centrer);
            Console.WriteLine("Voulez-vous recommencer une partie ? ");
            Console.Write(centrer);
            Console.Write("O = oui / N = non / G = afficher la grille  : ");
            char ans = char.Parse(Console.ReadLine().ToLower()); // Pour éviter la casse 
            if (ans == 'o')
            {
                Console.Clear();
                Run();
            }
            else
            {
                if (ans == 'n')
                {
                    Environment.Exit(0);
                }
                if (ans == 'g')
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Voici la grille de jeu :");
                    Console.ResetColor();
                    Console.WriteLine();
                    AfficherGrille(grille);
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.Write(">>>>> ");
                    ans = char.Parse(Console.ReadLine().ToLower());
                    Console.ResetColor();
                    if (ans == 'o')
                    {
                        Console.Clear();
                        Run();
                    }
                    else
                    {
                        if (ans == 'n')
                        {
                            Environment.Exit(0);
                        }
                    }
                    Console.ReadKey();

                }
            }

        }
        static void AnimerVictoire(int compteurTours)
        // Affiche la page de fin de la partie, lorsque gagnée;
        {
            Console.WriteLine();

            Console.WriteLine();
            System.Media.SoundPlayer gameWon = new System.Media.SoundPlayer(Properties.Resources.sonVictory);
            gameWon.Play();

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Clear(); //Console.Width

            string centrer = new string(' ', Console.WindowHeight / 2); //variable pour centrer le texte;
            // Affichage du message "Victory"
            Console.Write(centrer);
            Console.WriteLine(@" __  __              __                              __     ");
            Console.Write(centrer);
            Console.WriteLine(@"/\ \/\ \  __        /\ \__                          /\ \    ");
            Console.Write(centrer);
            Console.WriteLine(@"\ \ \ \ \/\_\    ___\ \ ,_\   ___   _ __   __  __   \ \ \   ");
            Console.Write(centrer);
            Console.WriteLine(@" \ \ \ \ \/\ \  /'___\ \ \/  / __`\/\`'__\/\ \/\ \   \ \ \  ");
            Console.Write(centrer);
            Console.WriteLine(@"  \ \ \_/ \ \ \/\ \__/\ \ \_/\ \L\ \ \ \/ \ \ \_\ \   \ \_\ ");
            Console.Write(centrer);
            Console.WriteLine(@"   \ `\___/\ \_\ \____\\ \__\ \____/\ \_\  \/`____ \   \/\_\");
            Console.Write(centrer);
            Console.WriteLine(@"    `\/__/  \/_/\/____/ \/__/\/___/  \/_/   `/___/> \   \/_/");
            Console.Write(centrer);
            Console.WriteLine(@"                                               /\___/       ");
            Console.Write(centrer);
            Console.WriteLine(@"                                               \/__/        ");


            Console.WriteLine();
            string centrer2 = new string(' ', Console.WindowHeight / 2 + 7); //variable pour centrer le petit texte;
            // Ecriture du petit texte
            Console.Write(centrer2);
            Console.WriteLine("HOURRA  ! Victoire petit pirate ! ");
            Console.Write(centrer2);
            Console.WriteLine($"Il t'a fallu {compteurTours} tours pour gagner :P ");
            // La partie est finie car tu as récupéré tous les trésors et découvert toutes les cases ne comportant pas de mines
            Console.Write(centrer2);
            Console.WriteLine("Veux-tu recommencer une partie ? ");
            Console.Write(centrer2);
            Console.WriteLine("O = oui / N = non");
            char ans = char.Parse(Console.ReadLine().ToLower());
            if (ans == 'o')
            {
                Console.Clear();
                Run();
            }
            else
            {
                if (ans == 'n')
                {
                    Environment.Exit(0);
                }
            }
        }

        static void AfficherTitre()
        // Affiche le titre avec les trois mots de couleurs différentes
        {
            // Affiche Le titre Chasse Au Tresor avec les trois couleurs
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"                 _____ _                         ");
            Console.ResetColor();
            Console.Write("  ___        ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("  _                            ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"                /  __ \ |                       ");
            Console.ResetColor();
            Console.Write(@"  / _ \        ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("| |                           ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"                | /  \/ |__   __ _ ___ ___  ___ ");
            Console.ResetColor();
            Console.Write(@" / /_\ \_   _  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("| |_ _ __ ___  ___  ___  _ __ ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"                | |   | '_ \ / _` / __/ __|/ _ \ ");
            Console.ResetColor();
            Console.Write(@"|  _  | | | |");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@" | __| '__/ _ \/ __|/ _ \| '__|");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"                | \__/\ | | | (_| \__ \__ \  __/");
            Console.ResetColor();
            Console.Write(@" | | | | |_| | ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"| |_| | |  __/\__ \ (_) | |   ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(@"                \_____/_| |_|\__,_|___/___/\___| ");
            Console.ResetColor();
            Console.Write(@"\_| |_/\__,_|  ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"\__|_|  \___||___/\___/|_|   ");
            Console.ResetColor();
        }
    }
}
