# Unity City Generator

Unity Editor Tool of a city generator.

Made by Javier Bullrich, Juan Cruz Araujo and Ivan Ramos

## Funcionamiento de Apple

Apple es una clase que contiene toda una manzana, representada como una array de lots (`Lot.cs`).

El metodo `BuildApple` tiene que generar una manzana usando la informacion en cada lote. Esta manzana, un conjunto de GameObjects tiene que tener un gameobject como padre (spawnea cada edificio y llama `MiGameObject.transform.setParent(gameobjectQueVaASerPadre.transform)`) y devuelve ese GameObject que contiene a los edificios como hijos, de manera tal que si se quiere recrear una manazana solamente se destruye ese gameObject.

Apple tiene dos variables, un Vector2 llamado `size` que marca la cantidad de casilleros que ocupa y un array de lots llamado `appleLots`.

La distancia entre dos casilleros se calcula usando `BuildCity.buildingFootprint`.

Por ejemplo, para calcular la distancia de 4 casilleros se puede usar algo como el siguiente metodo:
```chasrp
// tomando en cuenta que size es 2,2
Vector3 physicalSize = new Vector3(size.x * BuildCity.buildingFootprint, 0, size.z * BuildCity.buildingFootprint);
```

### Lot

La clase lot es una clase que implementa la interfaz ILot.

```csharp
public class Lot : ILot
{
    public Vector2 gridPos { get; private set; } // posicion dentro de la grilla
    public Vector3 worldPos { get; set; } // posicion fisica en el mundo
    public GameObject[] buildings { get; private set; } //array de eficios disponibles para spawnear
    public Lot neighboor; // vecino. Se usa para calcular la manzana.

    // constructor y etc
}
```

Lot contiene un array de edificios que puede construir. La idea seria que si hay un edificio grande, que ocupa 4 casilleros, y justo 4 casilleros vecinos tienen el mismo array de edificios, spwanear ese edificio grande en el medio, ocupando los cuatro casilleros.

[Como obtener el tamano de un GameObject (no testeado)](http://answers.unity3d.com/answers/927824/view.html)

## Custom Inspector

El custom Inspector de city manajer tiene que tener los siguientes valores:

 * Tres fields para gameobjects, llamados: X Streets, Z Streets y Cross Road.
 * Dos fields para int llamados Map Width y Map Height. Si los podes poner para que se vea como un field para un vector 2 genial (los dos en la misma linea.). Solo tienen que aceptar valores positivos.
 * Un array de neighboorhood:
 * * Un range con un valor int llamado Chance to Appear y un array de gameobjects llamado Buildings.

Fijate como esta armado ahora el inspector, algo asi pero mas lindo.

Tiene que haber una info box de error arriba del inspector si:
 * Alguno de los fields de gameobjects esta vacio.
 * Map Width o Map Height tiene como valor 0.
 * Neighboorhood no tiene ningun valor.
 * * Neighboorhood.buildings no tiene ningun valor.
 * * Si neighboorhood.ChanceToAppear es 0, deberia tener un warning justo abajo del valor.
