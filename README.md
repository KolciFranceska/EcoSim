Auteurs : Françeska Kolci (195387) et Yllke Prebreza (18402)

Dans ce README vous trouverez les éléments suivants : Le diagramme de classes, le diagramme de séquence, les explications des principes SOLID utilisés dans notre projet.

1. Le diagramme de classes

<img width="517" alt="image" src="https://user-images.githubusercontent.com/99248711/211172938-9ad6d9a0-d2a6-46b8-9282-49f5cff1577d.png">

2. Le diagramme de séquences

<img width="497" alt="image" src="https://user-images.githubusercontent.com/99248711/211172952-f292eda4-55d4-4ffa-9b00-c3a338b60649.png">

3. Les princcipes SOLID

Deux des principes SOLID respectés dans notre projet sont le principe de responsabilité unique (SRP) et le principe d’interface unique (ISP).

A. Principe de responsabilité unique :

Le principe de responsabilité unique stipule qu'une classe ou un module doit avoir une seule responsabilité, c'est-à-dire qu'elle ne doit effectuer qu'une seule tâche. Cela permet de maintenir un code lisible et facile à maintenir, car chaque classe a une fonction clairement définie et ne s'occupe que de cette tâche.

Dans notre projet, la classe LifeForm est responsable de la gestion de base de toutes les formes de vie dans l'environnement, tels que la santé, l'énergie et la mobilité. Elle est également responsable de la mise à jour de ces valeurs et de la mort des formes de vie. Cela respecte le principe de responsabilité unique, car elle n'a qu'une seule responsabilité : gérer les formes de vie de base.

En revanche, si la classe LifeForm avait été chargée de la gestion de l'environnement dans son ensemble (par exemple, la gestion de la nourriture et de l'espace disponible), elle aurait eu plusieurs responsabilités, ce qui ne respecterait pas le principe de responsabilité unique.

B. Principe d'interface unique :

Le principe d'interface unique stipule qu'une interface ne doit contenir que les méthodes qui sont nécessaires pour ses utilisateurs. Cela permet d'éviter les interfaces trop complexes et difficiles à utiliser, et de s'assurer que chaque classe qui implémente une interface ne doit se soucier que des méthodes dont elle a besoin.

Dans notre projet, la classe LifeForm définit une interface avec les méthodes Update, Feed, Reproduce et Die. Ces méthodes sont nécessaires pour toutes les formes de vie, et ne contiennent pas de méthodes inutiles pour certaines formes de vie. La classe Animal, qui hérite de LifeForm, n'a besoin que de ces méthodes pour fonctionner correctement, et n'est pas chargée de méthodes superflues. Cela respecte le principe d'interface unique.

Par exemple, si la classe LifeForm avait défini une méthode FindPartner qui n'est utilisée que par les animaux, cela aurait violé le principe d'interface unique, car cette méthode ne serait pas nécessaire pour toutes les formes de vie. En revanche, en la déplaçant dans la classe Animal, qui hérite de LifeForm, le principe d'interface unique est respecté, car cette méthode ne sera utilisée que par les animaux et ne sera pas présente dans l'interface de LifeForm.




Remarque :

Par manque de temps, nous n'avons pas pu terminer le projet. Nous avons créer toutes les classes et fonctions dans le fichier MainPage mais malheureusement nous n'avons pas eu le temps de travailler correctement sur l'interface.
