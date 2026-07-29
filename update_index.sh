#!/bin/bash
ENTITIES=("Categories" "Authors" "Publishers" "Magazines" "Newspapers")

for PLURAL in "${ENTITIES[@]}"; do
    FILE="LMSystem.Web/Views/${PLURAL}/Index.cshtml"
    
    # We will use sed to insert a th for Actions and td for the links
    # But wait, sed is not recommended per EPHEMERAL_MESSAGE. 
    # Actually, it's just faster to rewrite them using my python script approach or write_to_file.
done
