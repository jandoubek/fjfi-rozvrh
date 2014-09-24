# Projekt MůjRozvrh FJFI

## Řešení:
Ostré nasazení na mujrozvrh.fjfi.cvut.cz

## Zadání:
* web aplikace sloužící studentům a učitelům pro nalezení, úpravu a export vlastního rozvrhu
* zdrojem dat XML soubory z programu TimeTableBuilder od Milana Krbálka
* zobrazení rozvrhu v zažitém designu
* nebude klást vysoké nároky na správu (nepřidělá nikomu práci, pouze naopak)
* poběží na školním serveru (.NET4/IIS Web Server)

## Funkcionality
* filtrování podle všech myslitelných vlastností
* zobrazení rozvrhu v zažitém designu
* ruční úprava zobrazených hodin (změna času, předmětu, jména učitele, ...)
* export do mnoha formátů: iCal, PDF, JPG, SVG
* možnost uložit si vytvořený rozvrh pro další použití

## Návrhy na upgrade 
* fulltextové vyhledávání hodin
* export do JPG
* mobilní verze webu
* uživatelské účty s přihlašováním přes KOS pro persistenci dat
* dostupnost databází i pro minulé semestry, možnost si je vybrat
* tooltip pro položky seznamu ve filtru

## Chyby
* zlepšit/upravit/zdokonalit (pro širší rozsah rozlišení) layout
* aby se po výběru položky v seznamu, neroloval seznam (ve filtrech) nahoru
* v dialogovém okně úpravy se v seznamech nezobrazuje aktuální položka
* chybí tutorialy
* dohlédnout na kvalitu dat - háčky, čárky, barvy, jména předmětů

## Team
* David Blatský - zadání, aplikační logika, datový model, detaily GUI, odpovědný za chod
* Václav Honzík - moduly pro export do iCal, PDF, SVG, JPG
* Oldřich Štika - vývoj GUI, javascript
* Tomáš Smola - testování, vývoj GUI
* Richard Salač - propojení modulů v controlleru, komunikace se správcem serveru

* Milan Krbálek - uživatel-správce aplikace, vkládá nový databázový soubor
* Jan Doubek - přednášejí předmětu PVS, při němž byl projek úspěšně rozběhnut
