using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;
using KModkit;

public class MischboozlScript : MonoBehaviour
{
    public KMBombModule Module;
    public KMBombInfo BombInfo;
    public KMAudio Audio;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _moduleSolved;

    public MischboozlCell[] Cells;
    public Material[] ColorMats;
    private KMSelectable[] Sels = new KMSelectable[16];
    public KMSelectable PlayPauseSel;
    public KMSelectable SubmitSel;
    public KMSelectable[] ArrowSels;
    public TextMesh PlayPauseText;
    public GameObject[] SelectorObjs;

    private static readonly bool[][][][] _boozleglyphs = new bool[26][][][]
    {
        new bool[3][][] // A
        {
            "####;####;####;####;##..;####;####;#..#;.##.;####;####;..##;####;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;.##.;####;####;.##.;####;#..#;####;####;#..#;....;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;....;....;##..;####;####;####;.##.;####;####;####;####;#..#;....;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // B
        {
            ".##.;####;####;####;####;#..#;##..;####;####;..##;.##.;####;####;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;##..;#..#;####;####;.##.;..##;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;####;####;####;#..#;####;####;####;####;.##.;####;####;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // C
        {
            "####;####;####;..##;####;....;##..;####;....;....;....;####;....;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;..##;####;....;##..;####;####;..##;.##.;####;####;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;####;..##;####;####;####;####;....;....;....;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // D
        {
            "....;.##.;####;####;.##.;####;#..#;####;####;####;....;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;..##;.##.;####;##..;####;####;#..#;....;##..;#..#;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;....;....;.##.;####;####;.##.;####;#..#;####;####;#..#;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // E
        {
            "####;####;####;####;####;....;....;####;####;....;....;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;....;....;####;####;....;....;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;.##.;..##;####;####;####;####;####;##..;####;####;#..#;....;##..;#..#;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // F
        {
            "####;####;####;####;....;....;....;####;....;....;....;####;....;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;....;....;####;####;....;####;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;.##.;..##;....;####;####;####;..##;##..;####;####;####;....;##..;#..#;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // G
        {
            ".##.;####;####;..##;####;#..#;##..;####;....;....;.##.;####;....;....;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;####;..##;####;#..#;##..;####;####;####;.##.;####;....;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;....;####;####;####;..##;####;####;##..;#..#;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // H
        {
            "....;####;....;....;####;####;####;####;....;####;....;....;....;####;....;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;..##;.##.;..##;####;####;####;####;####;####;####;####;##..;#..#;##..;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;....;####;####;####;....;####;####;####;####;....;####;####;####;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // I
        {
            "....;....;.##.;####;....;.##.;####;#..#;.##.;####;####;..##;####;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;....;....;##..;####;####;..##;....;####;####;####;....;##..;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;....;.##.;..##;####;####;####;####;####;####;####;####;....;....;##..;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // J
        {
            "####;....;####;####;####;....;....;####;####;....;....;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;....;....;##..;####;####;####;....;####;####;####;....;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;..##;....;....;####;####;..##;.##.;#..#;##..;####;####;....;....;##..;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // K
        {
            "####;####;####;..##;##..;#..#;##..;####;.##.;..##;.##.;####;####;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;.##.;####;####;.##.;####;####;#..#;##..;####;####;..##;....;##..;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;####;....;####;####;####;####;####;####;####;####;....;####;####;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // L
        {
            "....;....;.##.;..##;....;....;##..;####;.##.;..##;....;####;##..;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;....;....;##..;####;####;....;....;####;####;..##;....;....;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;....;....;....;####;####;####;####;####;####;####;####;....;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // M
        {
            "####;####;####;####;####;....;....;####;####;..##;.##.;####;##..;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;.##.;####;####;####;####;####;####;..##;.##.;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;.##.;..##;....;####;####;####;####;####;####;####;####;##..;#..#;##..;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // N
        {
            ".##.;####;####;..##;####;#..#;##..;####;####;....;.##.;####;####;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;.##.;####;####;####;####;####;####;#..#;##..;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;.##.;..##;....;####;####;####;####;####;####;####;####;....;##..;#..#;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // O
        {
            "####;..##;.##.;####;####;####;####;####;####;....;....;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;####;####;..##;####;....;##..;####;####;..##;....;####;##..;####;####;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;####;####;####;####;....;....;####;####;....;....;####;####;####;####;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // P
        {
            ".##.;####;####;..##;####;#..#;##..;#..#;####;....;....;....;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;####;..##;####;#..#;##..;####;####;....;.##.;####;####;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;....;####;####;####;....;####;####;#..#;....;####;#..#;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // Q
        {
            "####;####;####;####;####;....;....;####;####;..##;.##.;####;##..;#..#;##..;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;....;####;####;....;....;....;####;....;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;####;####;####;#..#;....;####;####;....;####;####;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // R
        {
            "####;####;####;####;####;....;.##.;####;####;....;##..;#..#;####;####;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;....;....;....;####;.##.;####;####;####;####;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;####;####;..##;####;....;##..;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // S
        {
            "####;....;.##.;..##;####;.##.;####;####;####;####;#..#;####;##..;#..#;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "..##;....;....;.##.;####;..##;.##.;####;####;#..#;##..;####;#..#;....;....;##..".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;....;....;####;####;....;....;....;....;####;####;....;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // T
        {
            ".##.;..##;.##.;####;##..;####;####;#..#;.##.;####;####;..##;####;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;.##.;..##;....;....;####;####;....;####;####;####;####;##..;####;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;....;####;####;####;..##;.##.;####;####;#..#;##..;####;....;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // U
        {
            "####;####;####;####;####;#..#;##..;####;####;....;....;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;..##;####;....;##..;####;####;....;....;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;####;####;....;##..;####;####;..##;....;####;####;####;####;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // V
        {
            ".##.;####;####;..##;####;#..#;##..;####;####;....;....;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;..##;####;##..;####;####;####;....;##..;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;..##;####;....;##..;####;####;..##;....;####;##..;####;####;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // W
        {
            ".##.;####;####;..##;####;####;####;####;####;##..;#..#;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            ".##.;####;....;....;####;####;####;....;....;####;####;####;....;....;####;#..#".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;####;..##;####;....;####;####;####;####;....;####;##..;####;####;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // X
        {
            "####;..##;.##.;..##;##..;####;####;#..#;.##.;####;####;..##;##..;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;....;####;....;####;####;####;####;####;####;....;####;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;....;####;####;####;....;#..#;####;####;.##.;....;####;####;####;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // Y
        {
            "....;.##.;..##;....;.##.;####;####;..##;####;#..#;##..;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;..##;....;....;##..;####;####;..##;.##.;####;####;#..#;####;#..#;....;....".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "....;####;..##;....;####;####;####;..##;##..;####;####;####;....;##..;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        },
        new bool[3][][] // Z
        {
            "####;..##;.##.;..##;####;####;####;####;####;####;####;####;##..;#..#;##..;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;....;....;####;####;..##;.##.;####;####;#..#;##..;####;####;....;....;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray(),
            "####;####;....;....;####;####;..##;....;....;##..;####;####;....;....;####;####".Split(';').Select(i => i.Select(j => j == '#').ToArray()).ToArray()
        }
    };
    private int[] _offsets = new int[3];
    private int[] _colors = new int[3];
    private int[] _shuffles = new int[16];
    private int _currentIx;
    private Coroutine _cycleGlyphs;
    private bool _canPressArrows;
    private int? _prevSelectedCell;

    private void Start()
    {
        _moduleId = _moduleIdCounter++;
        for (int i = 0; i < Cells.Length; i++)
        {
            Sels[i] = Cells[i].GetComponent<KMSelectable>();
            Sels[i].OnInteract += CellPress(i);
        }
        for (int i = 0; i < ArrowSels.Length; i++)
            ArrowSels[i].OnInteract += ArrowPress(i);
        PlayPauseSel.OnInteract += PlayPausePress;
        SubmitSel.OnInteract += SubmitPress;
        _colors = Enumerable.Range(0, 3).ToArray().Shuffle();
        _offsets = Enumerable.Range(0, 26).ToArray().Shuffle().Take(3).ToArray();
        _shuffles = Enumerable.Range(0, 16).ToArray().Shuffle();
        _cycleGlyphs = StartCoroutine(CycleGlyphs());
        Debug.LogFormat("[Mischboozl #{0}] Shuffled order: {1}", _moduleId, _shuffles.Select(i => i + 1).ToArray().Join(" "));
        Debug.LogFormat("[Mischboozl #{0}] Set A is in color {1}, Set B is in color {2}, Set C is in color {3}.", _moduleId, Array.IndexOf(_colors, 0) == 0 ? "red" : Array.IndexOf(_colors, 0) == 1 ? "green" : "blue", Array.IndexOf(_colors, 1) == 0 ? "red" : Array.IndexOf(_colors, 1) == 1 ? "green" : "blue", Array.IndexOf(_colors, 2) == 0 ? "red" : Array.IndexOf(_colors, 2) == 1 ? "green" : "blue");
    }

    private KMSelectable.OnInteractHandler ArrowPress(int i)
    {
        return delegate ()
        {
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
            if (_moduleSolved || !_canPressArrows)
                return false;
            if (i == 0)
                _currentIx = (_currentIx + 25) % 26;
            else
                _currentIx = (_currentIx + 1) % 26;
            SetGlyphs();
            return false;
        };
    }

    private KMSelectable.OnInteractHandler CellPress(int i)
    {
        return delegate ()
        {
            if (_moduleSolved)
                return false;
            PlayRandomSound();
            if (_prevSelectedCell == null)
            {
                _prevSelectedCell = i;
                SelectorObjs[i].SetActive(true);
            }
            else
            {
                var temp = _shuffles[i];
                _shuffles[i] = _shuffles[_prevSelectedCell.Value];
                _shuffles[_prevSelectedCell.Value] = temp;
                var a = new List<int>() { i, _prevSelectedCell.Value };
                a.Sort();
                Debug.LogFormat("[Mischboozl #{0}] Swapped cells {1} and {2}.", _moduleId, a[0] + 1, a[1] + 1);
                Debug.LogFormat("[Mischboozl #{0}] Order is now: {1}.", _moduleId, _shuffles.Select(j => j + 1).Join(" "));
                foreach (var obj in SelectorObjs)
                    obj.SetActive(false);
                _prevSelectedCell = null;
            }
            SetGlyphs();
            return false;
        };
    }

    private bool PlayPausePress()
    {
        if (_moduleSolved)
            return false;
        if (!_canPressArrows)
        {
            if (_cycleGlyphs != null)
                StopCoroutine(_cycleGlyphs);
            _canPressArrows = true;
            PlayPauseText.text = "PLAY";
        }
        else
        {
            _cycleGlyphs = StartCoroutine(CycleGlyphs());
            _canPressArrows = false;
            PlayPauseText.text = "PAUSE";
        }
        return false;
    }

    private bool SubmitPress()
    {
        bool correct = true;
        for (int i = 0; i < _shuffles.Length; i++)
            if (i != _shuffles[i])
                correct = false;
        Debug.LogFormat("[Mischboozl #{0}] Submitted order: {1}.", _moduleId, _shuffles.Select(i => i + 1).Join(" "));
        if (!correct)
        {
            Module.HandleStrike();
            Debug.LogFormat("[Mischboozl #{0}] Not all cells were correctly sorted. Strike.", _moduleId);
            return false;
        }
        if (_cycleGlyphs != null)
            StopCoroutine(_cycleGlyphs);
        StartCoroutine(SolveAnimation());
        Debug.LogFormat("[Mischboozl #{0}] All cells were correctly sorted. Module solved.", _moduleId);
        return false;
    }

    private void SetGlyphs()
    {
        for (int i = 0; i < Cells.Length; i++)
            for (int j = 0; j < 4; j++)
                Cells[i].Semgents[j].GetComponent<MeshRenderer>().material = ColorMats[((_boozleglyphs[(_currentIx + _offsets[0]) % 26][_colors[0]][_shuffles[i]][j] ? 4 : 0) + (_boozleglyphs[(_currentIx + _offsets[1]) % 26][_colors[1]][_shuffles[i]][j] ? 2 : 0) + (_boozleglyphs[(_currentIx + _offsets[2]) % 26][_colors[2]][_shuffles[i]][j] ? 1 : 0))];
    }

    private IEnumerator CycleGlyphs()
    {
        while (true)
        {
            SetGlyphs();
            yield return new WaitForSeconds(0.75f);
            _currentIx = (_currentIx + 1) % 26;
        }
    }

    private IEnumerator SolveAnimation()
    {
        _moduleSolved = true;
        Audio.PlaySoundAtTransform("Solve", transform);
        for (int i = 0; i < Cells.Length; i++)
            for (int j = 0; j < 4; j++)
                Cells[i].Semgents[j].GetComponent<MeshRenderer>().material = ColorMats[0];
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < Cells.Length; i++)
            for (int j = 0; j < 4; j++)
                Cells[i].Semgents[j].GetComponent<MeshRenderer>().material = ColorMats[(_boozleglyphs[15][0][_shuffles[i]][j] ? 4 : 0) + (_boozleglyphs[14][1][_shuffles[i]][j] ? 2 : 0) + (_boozleglyphs[6][2][_shuffles[i]][j] ? 1 : 0)];
        Module.HandlePass();
    }

    private void PlayRandomSound()
    {
        Audio.PlaySoundAtTransform("SegSelect" + Rnd.Range(1, 6), transform);
    }

#pragma warning disable 0414
    private readonly string TwitchHelpMessage = "!{0} swap 1 16 [Swap cells 1 and 16. Cells are in reading order.] | !{0} toggle [Pauses/resumes the cycle] | !{0} left/right <#> [Cycle left/right in the sequence, optionally with amount] | !{0} submit [Submit the answer]";
#pragma warning restore 0414

    // Taken from Faulty 14 Segment Display's Twitch Plays support.

    private abstract class TpCommand { }
    private sealed class TpSwap : TpCommand { public int Cell1, Cell2; }
    private sealed class TpToggle : TpCommand { }
    private sealed class TpMove : TpCommand { public bool Right; public int Amount; }
    private sealed class TpSubmit : TpCommand { }

    private IEnumerator ProcessTwitchCommand(string command)
    {
        var commandPieces = command.ToLowerInvariant().Split(';');
        var commands = new List<TpCommand>();
        Match m;

        foreach (var cmd in commandPieces)
        {
            if ((m = Regex.Match(cmd, @"^\s*swap\s*(\d+)\s*(\d+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
            {
                int val1;
                int val2;
                if (!int.TryParse(m.Groups[1].Value, out val1) || !int.TryParse(m.Groups[2].Value, out val2))
                {
                    yield return "sendtochaterror Invalid segments! Must be in the range from 1 to 16";
                    yield break;
                }
                if (val1 > 16 || val2 > 16 || val1 < 1 || val2 < 1)
                {
                    yield return "sendtochaterror Invalid segments! Must be in the range from 1 to 16";
                    yield break;
                }
                commands.Add(new TpSwap { Cell1 = val1, Cell2 = val2 });
                continue;
            }
            if ((m = Regex.Match(cmd, @"^\s*(left|(?<r>right))(?<amtopt>\s+(?<amt>\d+))?\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
            {
                int amount = 1;
                if (m.Groups["amtopt"].Success && (!int.TryParse(m.Groups["amt"].Value, out amount) || amount < 1 || amount > 26))
                {
                    yield return string.Format("sendtochaterror “{0}” is an invalid amount by which to move left or right (must be 1–26).", m.Groups["amt"].Value);
                    yield break;
                }
                commands.Add(new TpMove { Right = m.Groups["r"].Success, Amount = amount });
                continue;
            }
            if ((m = Regex.Match(cmd, @"^\s*(pause|play|resume|toggle)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
            {
                commands.Add(new TpToggle());
                continue;
            }
            if ((m = Regex.Match(cmd, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
            {
                commands.Add(new TpSubmit());
                continue;
            }
            yield break;
        }
        yield return null;
        foreach (var cmd in commands)
        {
            TpSwap swap;
            TpMove move;
            if ((swap = cmd as TpSwap) != null)
            {
                Sels[swap.Cell1 - 1].OnInteract();
                yield return new WaitForSeconds(0.2f);
                Sels[swap.Cell2 - 1].OnInteract();
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            if ((move = cmd as TpMove) != null)
            {
                if (!_canPressArrows)
                {
                    yield return "sendtochaterror You can't go left or right if the sequence is cycling!";
                    yield break;
                }
                for (var i = 0; i < move.Amount; i++)
                {
                    (move.Right ? ArrowSels[1] : ArrowSels[0]).OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                continue;
            }
            if (cmd is TpToggle)
            {
                PlayPauseSel.OnInteract();
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            if (cmd is TpSubmit)
            {
                SubmitSel.OnInteract();
                yield return new WaitForSeconds(0.1f);
                continue;
            }
            yield break;
        }
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = 0; i < 16; i++)
        {
            if (_shuffles[i] == i)
                continue;
            Sels[i].OnInteract();
            yield return new WaitForSeconds(0.1f);
            Sels[Array.IndexOf(_shuffles, i)].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
        SubmitSel.OnInteract();
        while (!_moduleSolved)
            yield return true;
    }
}
