using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameCreator
{

    private static string[] maleName =
    {
        "Jhonny", "Bob", "Stuart", "Kevin", "Carl",
        "Marco", "Jean-Claud", "Fred", "Mario", "Lucas",
        "Paolo", "Abelardo", "Rodrigo", "Victor", "Teo",
        "Rodolfo", "Alphonse", "Joe", "Hansak", "Peter",
        "Mark", "Fausto", "Richard", "Michael", "Ken",

        "Joseph", "Xavier", "Romulo", "Ronnald", "Billy",
        "Willy", "Albert", "Hank", "Henry", "Jacob",
        "Edward", "Trevor", "Alph", "Antoinne", "Donald",
        "Nathan", "Timmy", "Jimmy", "Paul", "Luigi",
        "Oswald", "Xander", "Leonardo", "Miguel", "Aron"
    };

    private static string[] femaleName =
    {
        "Michelle", "Debora", "Sara", "Trina", "Noemi",
        "Vanessa", "Maria", "Vicky", "Olga", "Annette",
        "Lorena", "Laura", "Reina", "Nanci", "Hilda",
        "Gloria", "Penelope", "Phoebe", "Mia", "Seraphina",
        "Ursula", "Poppy", "Carla", "Jessica", "Ashley"
    };

    private static string[] familyName =
    {
        "Brown", "Rodriguez", "Garcia", "Jackson", "Jhonson",
        "Bryan", "Cueva", "Gil", "Mars", "Scarlet",
        "James", "Du-Pont", "Gomez", "Schrodringer", "Ryan",
        "Evans", "Black", "Zuckerberg", "XXX", "Turner",
        "Jools", "Jops", "Green", "Yoshi", "Reynolds",

        "Obama", "Nickson", "Tesla", "Edison", "Colon",
        "Rogers", "West", "North", "Claus", "Christmas",
        "Summer", "Bayern", "Avogadro", "Heisenberg", "Viyuela",
        "Romero", "Dudeman", "Marley", "Fridge", "Arnold",
        "Poppers", "Wolf", "Carbon", "Shower", "Kim"
    };

    private static string[] maleUniqueName =
    {
        "Joe Mama", "Ronald McDonald", "NoobMaster69", "Lil Timmy", "Mark Zuckerberg",
        "Carlos Ruiz", "Herctor Fernandez", "David Fontela", "Dan Casas", "Carlos Garre"
    };

    private static string[] femaleUniqueName =
    {
        "Luxanna Crownguard", "Marie Curie", "Jeanne d'Arc", "Kim Kardashian", "Michelle Obama",
        "Marta Sebastián"
    };

    public class PersonName
    {
        public string name;
        public bool isMale;
        public string description;

        public PersonName()
        {
            name = "";
            isMale = true;
            description = "";
        }
    }

    public static PersonName Generate()
    {
        PersonName p = new PersonName();
        if ((int) Random.Range(0,2) == 0)
        {
            p.isMale = false;
        }

        if ((int) Random.Range(0,50) == 0)
        {
            if (p.isMale)
            {
                p.name = maleUniqueName[(int)Random.Range(0, maleUniqueName.Length)];
            } else
            {
                p.name = femaleUniqueName[(int)Random.Range(0, femaleUniqueName.Length)];
            }
            return p;
        }

        if (p.isMale)
        {
            p.name = maleName[(int)Random.Range(0, maleName.Length)];
        } else
        {
            p.name = femaleName[(int)Random.Range(0, femaleName.Length)];
        }
        p.name += " " + familyName[(int)Random.Range(0, familyName.Length)];

        return p;
    }

}
