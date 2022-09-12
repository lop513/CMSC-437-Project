using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UserInput : MonoBehaviour
{
    
    public GameObject slot1;
    public GameObject game;
    private Solitaire solitaire;
    private UIButtons UIButtons;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }

        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        UIButtons = FindObjectOfType<UIButtons>();
        if (!UIButtons.isPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickCount++;


                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit)
                {
                    //slot1 = game;
                    if (hit.collider.CompareTag("Deck")) // click deck
                    {
                        //slot1 = game;
                        slot1 = this.gameObject;
                        Deck();
                    }
                    else if (hit.collider.CompareTag("Card")) // click card
                    {

                        Card(hit.collider.gameObject);
                    }
                    else if (hit.collider.CompareTag("Top")) // click top
                    {

                        Top(hit.collider.gameObject);
                    }
                    else if (hit.collider.CompareTag("Bottom")) // click bottom
                    {

                        Bottom(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    void Deck()
    {
        print("Clicked on Deck");
        solitaire.DealFromDeck();
        slot1 = this.gameObject;
    }

    void Card(GameObject selected)
    {
       
        print("Clicked on Card");


        if (!selected.GetComponent<Selectable>().faceUp)
        {
            if (!Blocked(selected))
            {
                selected.GetComponent<Selectable>().faceUp = true;
                slot1 = this.gameObject;
            }
        }
        else if (selected.GetComponent<Selectable>().inDeckPile)
        {
            if (!Blocked(selected))
            {
                if (slot1 == selected)
                {
                    if (DoubleClick())
                    {
                        AutoStack(selected);
                    }
                }
                else
                {
                    slot1 = selected;
                }
            }
        }
        else
        {
            if (slot1 == this.gameObject)
            {
                slot1 = selected; // snot null b/c we pass in gameObject 
            }

            else if (slot1 != selected)
            {
                if (Stackable(selected))
                {
                    Stack(selected);
                }
                else
                {
                    slot1 = selected;
                }
            }

            else if (slot1 == selected)
            {
                if (DoubleClick())
                {
                    AutoStack(selected);
                }
            }
        }
    }

    void Top(GameObject selected)
    {
       
        print("Clicked on Top");
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 1) // if card is an ace adn empty slot then stack
            {
                Stack(selected);
            }
        }

    }

    void Bottom(GameObject selected)
    {

        print("Clicked on Bottom");

        // if card is king and empty slot is bottom then stack

        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }

    bool Stackable(GameObject selected)
    {
        // Source
        Selectable s1 = slot1.GetComponent<Selectable>();

        // Destination
        Selectable s2 = selected.GetComponent<Selectable>();

        if (!s2.inDeckPile)
        {
            if (s2.top)
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }

                else
                {
                    return false;
                }
            }

            else
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        print("Not stackable");
                        return false;
                    }

                    else
                    {
                        print("Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        // If on top of king or empty bottom stack the cards in place
        // else stack the cards with a negative y offset

        // Source
        Selectable s1 = slot1.GetComponent<Selectable>();

        // Destination
        Selectable s2 = selected.GetComponent<Selectable>();


        float yOffset = 0.3f;

        if (s2.top || (!s2.top && s1.value == 13))
        {
            yOffset = 0;
        }

        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - .01f);
        slot1.transform.parent = selected.transform; // this makes the children move with the parents

        if (s1.inDeckPile) // removes the cards from the top pile to prevent duplicate cards
        {
            solitaire.tripsOnDisplay.Remove(slot1.name);
        }

        else if (s1.top && s2.top && s1.value == 1) // allows movement of cards between top spots
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = null;
        }

        else if (s1.top) // keeps track of the current value of the top decks as a card has been removed
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }

        else // removes the card string from the apporpriate bottom list
        {
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }

        s1.inDeckPile = false; // you cannot add cards to the trips pile to this is always fine
        s1.row = s2.row;

        if (s2.top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.top = true;
        }

        else
        {
            s1.top = false;
        }

        // after completing move reset slot1 to be essentially null as being null will break the logic
        slot1 = this.gameObject;
    }

    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitaire.tripsOnDisplay.Last())
            {
                return false;
            }
            else
            {
                print(s2.name + " is blocked by " + solitaire.tripsOnDisplay.Last()); // check if its bottom card
                return true;
            }
        }
        else
        {
            if (s2.name == solitaire.bottoms[s2.row].Last())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }

    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();

            if (selected.GetComponent<Selectable>().value == 1)
            {
                if (solitaire.topPos[i].GetComponent<Selectable>().value == 0)
                {
                    slot1 = selected;
                    Stack(stack.gameObject);
                    break;
                }
            }
            else
            {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        string lastCardName = stack.suit + stack.value.ToString();

                        if (stack.value == 1)
                        {
                            lastCardName = stack.suit + "A";
                        }

                        if (stack.value == 11)
                        {
                            lastCardName = stack.suit + "J";
                        }

                        if (stack.value == 12)
                        {
                            lastCardName = stack.suit + "Q";
                        }

                        if (stack.value == 13)
                        {
                            lastCardName = stack.suit + "K";

                        }

                        GameObject lastCard = GameObject.Find(lastCardName);
                        Stack(lastCard);
                        break;
                    }
                }
            }


        }
    }

    bool HasNoChildren(GameObject card)
    {
        int i = 0;


        foreach (Transform child in card.transform)
        {
            i++;
        }

        if (i == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



}


