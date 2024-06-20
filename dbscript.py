from collections.abc import MutableSet
from enum import Enum
import sqlite3
import requests
import logging


class PokeType(Enum):
    NORMAL = 0
    FIRE = 1
    WATER = 2
    ELECTRIC = 3
    GRASS = 4
    ICE = 5
    FIGHTING = 6
    POISON = 7
    GROUND = 8
    FLYING = 9
    PSYCHIC = 10
    BUG = 11
    ROCK = 12
    GHOST = 13
    DRAGON = 14
    DARK = 15
    STEEL = 16
    FAIRY = 17
    UNKNOWN = 18
    SHADOW = 19


class DamageClass(Enum):
    PHYSICAL = 0
    SPECIAL = 1
    STATUS = 2


DATABASE: str = "./pokedb.db"
BASE_URL: str = "https://pokeapi.co/api/v2/"
TYPES: MutableSet = set()

logger = logging.getLogger(__name__)


def main() -> int:
    logging.basicConfig(filename="dbscript.log")
    logger.info("Started")
    db = sqlite3.connect(DATABASE)
    cur = db.cursor()
    types(cur)
    moves(cur)
    db.commit()
    cur.close()
    logger.info("Finished")
    return 0


def types(cur: sqlite3.Cursor):
    logger.info("Started Types")
    for attacker in PokeType:
        result = requests.get(BASE_URL + "type/" + attacker.name.lower()).json()["damage_relations"]
        for defender in PokeType:
            mult = 1.0
            for t in result["double_damage_to"]:
                if t["name"] == defender.name.lower():
                    mult = 2.0
                    break
            for t in result["half_damage_to"]:
                if t["name"] == defender.name.lower():
                    mult = 0.5
                    break
            for t in result["no_damage_to"]:
                if t["name"] == defender.name.lower():
                    mult = 0.0
                    break
            cur.execute(
                "INSERT INTO matchups (attacker, defender, multiplier) VALUES (?,?,?)",
                (attacker.value, defender.value, mult),
            )
            logger.info(f"Inserted {(attacker.name, defender.name, mult)}")
    logger.info("Finished Types")


def moves(cur: sqlite3.Cursor):
    logger.info("Started Moves")
    results = requests.get(BASE_URL + "move/?limit=10000").json()["results"]
    logger.info(f"Fetched {len(results)}")
    for result in results:
        name: str = result["name"]
        url: str = result["url"]
        # Check if in db already
        cur.execute("SELECT COUNT(*) FROM moves WHERE name = ?", (name,))
        count = cur.fetchone()[0]
        # Request
        if count == 0:
            try:
                move_result = requests.get(url).json()
            except requests.JSONDecodeError:
                logger.error(f"Request for {name} failed")
            else:
                mvid = move_result["id"]
                name = move_result["name"]
                power = move_result["power"]
                pp = move_result["pp"]
                accuracy = move_result["accuracy"]
                mvtype = PokeType[move_result["type"]["name"].upper()].value
                damageClass = DamageClass[
                    move_result["damage_class"]["name"].upper()
                ].value
                cur.execute(
                    """INSERT INTO moves (id, name, power, pp, accuracy, type, damageClass)
                            VALUES (?,?,?,?,?,?,?)""",
                    (mvid, name, power, pp, accuracy, mvtype, damageClass),
                )
                logger.info(f"Inserted {name}")
    logger.info("Finished Moves")


if __name__ == "__main__":
    main()
