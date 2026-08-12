namespace LogicDetective.Tests;

public class HintTrackerTests
{
    [Fact]
    public void GetNext_ReturnsEachHintOnlyOnce()
    {
        var tracker = new HintTracker();
        var item1 = new Item(0, 0, "A");
        var item2 = new Item(1, 0, "X");
        var clue = new SameClue(item1, item2);
        var relations = new[] { clue };

        Assert.Same(clue, tracker.GetNext(relations));
        Assert.Null(tracker.GetNext(relations));
    }

    [Fact]
    public void GetNext_ReturnsNoHintWhenRelationsAreEmpty()
    {
        var tracker = new HintTracker();
        var result = tracker.GetNext(Array.Empty<Clue>());
        Assert.Null(result);
    }

    [Fact]
    public void GetNext_ReturnsHintsInOrder()
    {
        var tracker = new HintTracker();
        var item1 = new Item(0, 0, "A");
        var item2 = new Item(1, 0, "X");
        var thirdItem = new Item(1, 1, "Y");
        var clue1 = new SameClue(item1, item2);
        var clue2 = new DifferentClue(item1, thirdItem);
        var relations = new Clue[] { clue1, clue2 };

        Assert.Same(clue1, tracker.GetNext(relations));
        Assert.Same(clue2, tracker.GetNext(relations));
        Assert.Null(tracker.GetNext(relations));
    }

    [Fact]
    public void GetNext_ReturnsNewHintWhenRelationsAreUpdated()
    {
        var tracker = new HintTracker();
        var item1 = new Item(0, 0, "A");
        var item2 = new Item(1, 0, "X");
        var thirdItem = new Item(1, 1, "Y");
        var clue1 = new SameClue(item1, item2);
        var clue2 = new DifferentClue(item1, thirdItem);
        Assert.Same(clue1, tracker.GetNext([clue1]));

        var relations = new Clue[] { clue1, clue2 };
        Assert.Same(clue2, tracker.GetNext(relations));
    }

    [Fact]
    public void GetNext_ReturnsNullAfterAllHintsHaveBeenShown()
    {
        var tracker = new HintTracker();
        var item1 = new Item(0, 0, "A");
        var item2 = new Item(1, 0, "X");
        var thirdItem = new Item(1, 1, "Y");

        var clue1 = new SameClue(item1, item2);
        var clue2 = new DifferentClue(item1, thirdItem);
        var relations = new Clue[] { clue1, clue2 };

        Assert.Same(clue1, tracker.GetNext(relations));
        Assert.Same(clue2, tracker.GetNext(relations));
        Assert.Null(tracker.GetNext(relations));
    }
}