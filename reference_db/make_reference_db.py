import argparse
import sys

from src.create_db import create_db
from src.dump_ids import dump_ids


def main() -> int:
    parser = argparse.ArgumentParser()
    subp = parser.add_subparsers(
        dest="subparser_name",
        help="Your help message",
    )

    # Add commands
    create_p = subp.add_parser("create", help="Create DB")
    dump_p = subp.add_parser("dump", help="Dump Song Ids")

    # Add command line arguments to the commands
    create_p.add_argument("--recreate", action="store_true")
    create_p.add_argument("--start_at", type=int)
    args = parser.parse_args()
    match args.subparser_name:
        case "create":
            create_db(args.recreate, args.start_at)
            return 0
        case "dump":
            dump_ids()
            return 0
        case _:
            parser.print_help()
            return 1


if __name__ == "__main__":
    sys.exit(main())
